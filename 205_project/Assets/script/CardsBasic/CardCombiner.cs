// �ļ�·��: Assets/script/CardsBasic/CardCombiner.cs (�����޸��� - ��ȷ�����ӹ�ϵ)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCombiner : MonoBehaviour
{
    [Header("�M�ϔ�����")]
    [Tooltip("�������кϳ��䷽�� ScriptableObject")]
    [SerializeField] private CardsCombination combinationDatabase;
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0);

    [Header("UI����")]
    [Tooltip("����Ľ�����UIԤ�����ϵ�����")]
    [SerializeField] private GameObject combinationProgressPrefab;
    [Tooltip("����������ڿ��Ƹ�����λ��ƫ��")]
    [SerializeField] private Vector3 progressBarOffset = new Vector3(0, 1.5f, 0);

    [Header("������������")]
    [Tooltip("�¿��Ƶ����Ķ���ʱ��")]
    [SerializeField] private float spawnAnimationDuration = 0.3f;

    private CombinationProgressUI currentProgressBar;
    private Card card;
    private Coroutine combinationCoroutine;

    public bool isCombining { get; private set; } = false;
    private float combinationStartTime;
    private CardsCombinationRule currentCombinationRule;

    private void Awake()
    {
        card = GetComponent<Card>();
    }

    private void Update()
    {
        if (isCombining && currentProgressBar != null && currentCombinationRule != null && currentCombinationRule.time > 0)
        {
            float elapsedTime = Time.time - combinationStartTime;
            float progress = elapsedTime / currentCombinationRule.time;
            currentProgressBar.UpdateProgress(progress);
        }
    }

    public void CheckForCombination()
    {
        if (card.Stacker.Parent != null || isCombining) return;

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        if (matchedRule != null)
        {
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    public void CancelCombination()
    {
        if (combinationCoroutine != null)
        {
            StopCoroutine(combinationCoroutine);
            if (currentProgressBar != null)
            {
                Destroy(currentProgressBar.gameObject);
            }
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> initialIngredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        // --- �����ϳ����� ---
        if (rule.time > 0 && combinationProgressPrefab != null)
        {
            GameObject progressBarInstance = Instantiate(combinationProgressPrefab, transform.position + progressBarOffset, Quaternion.identity, transform);
            currentProgressBar = progressBarInstance.GetComponent<CombinationProgressUI>();
        }
        if (rule.time > 0) yield return new WaitForSeconds(rule.time);
        if (currentProgressBar != null) Destroy(currentProgressBar.gameObject);
        if (EffectManager.Instance != null) EffectManager.Instance.PlayCombinationEffect(transform.position);

        Vector3 rootPosition = transform.position;
        foreach (var result in rule.results)
        {
            if (Random.value <= result.probability)
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    if (result.resultCard.cardType == CardsBasicData.CardType.Ant && PopulationManager.Instance != null && PopulationManager.Instance.IsPopulationFull())
                    {
                        Debug.LogWarning("�ϳ�ʧ�ܣ���Ѩ�������޷������µ����ϣ�");
                        continue;
                    }
                    Card newCard = CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition);
                    if (newCard != null) StartCoroutine(AnimateCardSpawn(newCard, rootPosition + spawnOffset));
                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        yield return new WaitForSeconds(spawnAnimationDuration + 0.1f);

        // ====================���޸��߼���ʼ��====================

        // 1. ʶ��Ҫ���ٵĿ��ƺ��Ҵ�Ŀ���
        var cardsToDestroy = new HashSet<Card>();
        var tempIngredients = new List<Card>(initialIngredientCards);
        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                var foundCards = tempIngredients
                    .Where(c => requiredGroup.specificCard != null ? c.CardData == requiredGroup.specificCard : c.CardData.cardType == requiredGroup.cardType)
                    .Take(requiredGroup.requiredCount).ToList();

                foreach (var foundCard in foundCards)
                {
                    cardsToDestroy.Add(foundCard);
                    tempIngredients.Remove(foundCard);
                }
            }
        }
        var survivingCards = initialIngredientCards.Where(c => !cardsToDestroy.Contains(c)).ToList();

        // 2.���ؼ�������¶������Ҵ���ӿ��������뼴�������ٵĸ����ƶϿ�����
        foreach (var cardToDestroy in cardsToDestroy)
        {
            var childrenCopy = cardToDestroy.Stacker.GetChildren();
            foreach (var childStacker in childrenCopy)
            {
                Card childCard = childStacker.GetComponent<Card>();
                if (childCard != null && !cardsToDestroy.Contains(childCard))
                {
                    childStacker.DetachFromParent(); // �������������ӿ��Ƶ�Parent���ã�ʹ���Ϊ�µĸ�
                }
            }
        }

        // 3. ���ٿ���
        foreach (var cardToReturn in cardsToDestroy)
        {
            if (cardToReturn != null)
            {
                if (EffectManager.Instance != null) EffectManager.Instance.PlayDissipateEffect(cardToReturn.transform.position);
                StartCoroutine(ReturnCardToPoolAfterDelay(cardToReturn, 0.5f));
            }
        }

        // 4. ���õ�ǰ�ϳ�����״̬
        isCombining = false;
        combinationCoroutine = null;
        currentCombinationRule = null;

        yield return new WaitForSeconds(0.1f); // �ȴ�һС�����ȷ�����ٲ����ѿ�ʼ

        // 5.���ؼ���״̬�ؽ������¼��
        if (survivingCards.Count > 0)
        {
            // �ҵ������Ҵ����У������Ǹ����Ƶ���Щ������Ϊ���µĶ����ƶѣ�
            var newRoots = survivingCards.Where(c => c != null && c.gameObject.activeInHierarchy && c.Stacker.Parent == null).ToList();

            Debug.Log($"�ϳ���ϣ��ҵ� {newRoots.Count} ���µĶ����ƶѡ�");

            // ����Ҵ����γ��˶���������ƶѣ������������������
            if (newRoots.Count > 1)
            {
                // �ҵ���ײ�������Ϊ�ػ�
                var baseCard = newRoots.OrderBy(c => c.transform.position.y).First();
                // ���������µĸ����ѵ�������ػ���
                foreach (var root in newRoots)
                {
                    if (root != baseCard)
                    {
                        root.Stacker.StackOn(baseCard.Stacker.GetTopmostCardInStack());
                        yield return new WaitForSeconds(0.2f); // �ȴ��ѵ��������
                    }
                }
            }

            // ��󣬶������γɵ��Ǹ��ƶѵĸ�������һ���µĺϳɼ��
            var finalRoot = survivingCards.FirstOrDefault(c => c != null && c.gameObject.activeInHierarchy)?.Stacker.GetRoot();
            if (finalRoot != null)
            {
                Debug.Log($"���µĸ� [{finalRoot.name}] ���¼��ϳɡ�");
                finalRoot.GetComponent<Card>().Combiner.CheckForCombination();
            }
        }

        // ====================���޸��߼�������====================
    }

    private IEnumerator AnimateCardSpawn(Card cardToAnimate, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = cardToAnimate.transform.position;
        if (cardToAnimate.Dragger != null) cardToAnimate.Dragger.enabled = false;

        while (elapsedTime < spawnAnimationDuration)
        {
            float t = elapsedTime / spawnAnimationDuration;
            float easedT = 1 - (1 - t) * (1 - t);
            cardToAnimate.transform.position = Vector3.Lerp(startingPosition, targetPosition, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardToAnimate.transform.position = targetPosition;
        if (cardToAnimate.Dragger != null) cardToAnimate.Dragger.enabled = true;
    }

    private IEnumerator ReturnCardToPoolAfterDelay(Card cardToReturn, float delay)
    {
        if (cardToReturn == null) yield break;

        cardToReturn.gameObject.SetActive(false);
        yield return new WaitForSeconds(delay);

        if (cardToReturn != null)
        {
            cardToReturn.gameObject.SetActive(true);
            CardPool.Instance.Return(cardToReturn);
        }
    }

    public float GetRemainingTime()
    {
        if (isCombining && currentCombinationRule != null)
        {
            float elapsedTime = Time.time - combinationStartTime;
            return Mathf.Max(0, currentCombinationRule.time - elapsedTime);
        }
        return 0;
    }
}