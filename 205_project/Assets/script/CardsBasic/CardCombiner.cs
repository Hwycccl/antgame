// �ļ�·��: Assets/script/CardsBasic/CardCombiner.cs (���޸�)
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
        if (card.Stacker.Parent != null) return;
        // �����޸� #1: ������ںϳɣ���ȡ���ɵ�
        if (isCombining)
        {
            CancelCombination();
        }

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        if (matchedRule != null)
        {
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    // --- �����޸� #2: ������ӻؽ�׳�� CancelCombination �������� ---
    /// <summary>
    /// ���ⲿȡ����ǰ�ĺϳɹ���
    /// </summary>
    public void CancelCombination()
    {
        if (combinationCoroutine != null)
        {
            StopCoroutine(combinationCoroutine);

            if (currentProgressBar != null)
            {
                Destroy(currentProgressBar.gameObject);
            }

            // ��������״̬
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }

    // �ļ�·��: Assets/script/CardsBasic/CardCombiner.cs (���޸�)

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        if (rule.time > 0 && combinationProgressPrefab != null)
        {
            GameObject progressBarInstance = Instantiate(combinationProgressPrefab, transform.position + progressBarOffset, Quaternion.identity, transform);
            currentProgressBar = progressBarInstance.GetComponent<CombinationProgressUI>();
        }

        // --- �׶�1: ���жϵļ�ʱ�׶� ---
        if (rule.time > 0)
        {
            float timer = rule.time;
            while (timer > 0)
            {
                // ÿһ֡����鿨�ƶѵ��Ƿ�����ƻ�
                if (!IsStackIntact(ingredientCards))
                {
                    Debug.Log("�ϳɹ��̱�����϶������жϡ�");
                    CancelCombination(); // ��ȫ��ȡ���ϳ�
                    yield break;         // �˳�Э��
                }
                timer -= Time.deltaTime;
                yield return null; // �ȴ���һ֡
            }
        }

        // ��ִ��ǰ�����һ�μ�飬��ֹ�����һ֡������
        if (!IsStackIntact(ingredientCards))
        {
            Debug.Log("�ϳ���ִ��ǰһ�̱��жϡ�");
            CancelCombination();
            yield break;
        }

        // --- �׶�2: �����жϵ�ִ�н׶� ---
        // �������в���Ŀ��ƣ���ֹ�����ɺ����ٶ����ڼ����BUG
        foreach (var ingredient in ingredientCards)
        {
            if (ingredient != null && ingredient.Dragger != null)
            {
                ingredient.Dragger.enabled = false;
            }
        }

        // ���������
        if (currentProgressBar != null)
        {
            Destroy(currentProgressBar.gameObject);
        }

        // ������Ч
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCombinationEffect(transform.position);
        }

        // --- ��ʼ�����¿��� ---
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
                    if (newCard != null)
                    {
                        Vector3 targetPosition = rootPosition + spawnOffset;
                        StartCoroutine(AnimateCardSpawn(newCard, targetPosition));
                    }
                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }

        yield return new WaitForSeconds(spawnAnimationDuration + 0.1f);

        // --- ��ʼ���پɿ��� ---
        List<Card> cardsToDestroy = new List<Card>();
        List<Card> availableIngredients = new List<Card>(ingredientCards); // �⽫����Щ�������ٵĿ���
        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                List<Card> foundCards;
                if (requiredGroup.specificCard != null)
                {
                    foundCards = availableIngredients.Where(c => c.CardData == requiredGroup.specificCard).Take(requiredGroup.requiredCount).ToList();
                }
                else
                {
                    foundCards = availableIngredients.Where(c => c.CardData.cardType == requiredGroup.cardType).Take(requiredGroup.requiredCount).ToList();
                }
                cardsToDestroy.AddRange(foundCards);
                foreach (var card in foundCards)
                {
                    availableIngredients.Remove(card); // ��ʣ���б����Ƴ�
                }
            }
        }

        // ... (�����߼��Ͷ���ػ��գ��ⲿ�ֱ��ֲ���) ...
        foreach (var cardToReturn in cardsToDestroy.Distinct())
        {
            if (cardToReturn != null)
            {
                var childrenCopy = cardToReturn.Stacker.GetChildren();
                foreach (var childStacker in childrenCopy)
                {
                    if (!cardsToDestroy.Contains(childStacker.GetComponent<Card>()))
                    {
                        cardToReturn.Stacker.SafelyRemoveChild(childStacker);
                    }
                }
            }
        }

        foreach (var cardToReturn in cardsToDestroy.Distinct().Reverse())
        {
            if (cardToReturn != null)
            {
                if (EffectManager.Instance != null)
                {
                    EffectManager.Instance.PlayDissipateEffect(cardToReturn.transform.position);
                }
                cardToReturn.gameObject.SetActive(false);
                CardPool.Instance.StartCoroutine(ReturnCardToPoolAfterDelay(cardToReturn, 1.0f));
            }
        }


        // --- ����״̬������ʣ�࿨�� ---
        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;

        foreach (var remainingCard in availableIngredients)
        {
            if (remainingCard != null && remainingCard.gameObject.activeInHierarchy)
            {
                // ��������δ�����ٿ��Ƶ��϶�����
                if (remainingCard.Dragger != null)
                {
                    remainingCard.Dragger.enabled = true;
                }
                remainingCard.Stacker.UpdateStackVisuals();
            }
        }
    }
    private IEnumerator AnimateCardSpawn(Card cardToAnimate, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = cardToAnimate.transform.position;

        if (cardToAnimate.Dragger != null)
        {
            cardToAnimate.Dragger.enabled = false;
        }

        while (elapsedTime < spawnAnimationDuration)
        {
            float t = elapsedTime / spawnAnimationDuration;
            float easedT = 1 - (1 - t) * (1 - t);
            cardToAnimate.transform.position = Vector3.Lerp(startingPosition, targetPosition, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardToAnimate.transform.position = targetPosition;

        if (cardToAnimate.Dragger != null)
        {
            cardToAnimate.Dragger.enabled = true;
        }
    }

    private IEnumerator ReturnCardToPoolAfterDelay(Card cardToReturn, float delay)
    {
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
    /// <summary>
    /// ��������������ʼ�ĺϳ�ԭ���Ƿ񶼻��ڵ�ǰ�Ķѵ���
    /// </summary>
    private bool IsStackIntact(List<Card> originalIngredients)
    {
        List<Card> currentStack = card.Stacker.GetCardsInStack();
        // ��������Ƿ�һ�£��Լ�����ԭʼ�����Ƿ񶼻���
        if (currentStack.Count != originalIngredients.Count) return false;

        // Linq.Allȷ��originalIngredients�е�ÿһ��Ԫ�ض���currentStack��
        return originalIngredients.All(c => currentStack.Contains(c));
    }
}