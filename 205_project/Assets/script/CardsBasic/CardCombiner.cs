// �ļ�·��: Assets/script/CardsBasic/CardCombiner.cs (���պϲ���)

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
        // (����) ���½���������ʾ�߼�
        if (isCombining && currentProgressBar != null && currentCombinationRule != null && currentCombinationRule.time > 0)
        {
            float elapsedTime = Time.time - combinationStartTime;
            float progress = elapsedTime / currentCombinationRule.time;
            currentProgressBar.UpdateProgress(progress);
        }
    }

    /// <summary>
    /// ��鵱ǰ�ƶ��Ƿ�����ϳ�����
    /// </summary>
    public void CheckForCombination()
    {
        if (card.Stacker.Parent != null) return;
        if (isCombining) CancelCombination();

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        if (matchedRule != null)
        {
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        // (����) ��������ʾ������
        if (rule.time > 0 && combinationProgressPrefab != null)
        {
            GameObject progressBarInstance = Instantiate(combinationProgressPrefab, transform.position + progressBarOffset, Quaternion.identity, transform);
            currentProgressBar = progressBarInstance.GetComponent<CombinationProgressUI>();
        }

        yield return null;

        if (rule.time > 0)
        {
            yield return new WaitForSeconds(rule.time);
        }

        // (����) ���ٽ�����
        if (currentProgressBar != null)
        {
            Destroy(currentProgressBar.gameObject);
        }

        // (�ָ�) ���źϳ�����Ч��
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCombinationEffect(transform.position);
        }

        // --- ����ϳɽ�� (���޸ĺ�İ汾�����ֲ���) ---
        Vector3 rootPosition = transform.position;
        foreach (var result in rule.results)
        {
            if (Random.value <= result.probability)
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    if (result.resultCard.cardType == CardsBasicData.CardType.Ant)
                    {
                        if (PopulationManager.Instance != null && PopulationManager.Instance.IsPopulationFull())
                        {
                            Debug.LogWarning("�ϳ�ʧ�ܣ���Ѩ�������޷������µ����ϣ�");
                            continue;
                        }
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

        // --- �����޸�����������ӻ�ȱʧ�Ŀ��������߼� ---

        List<Card> cardsToDestroy = new List<Card>();
        List<Card> availableIngredients = new List<Card>(ingredientCards);

        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                List<Card> foundCards = new List<Card>();

                if (requiredGroup.specificCard != null)
                {
                    foundCards = availableIngredients
                        .Where(c => c.CardData == requiredGroup.specificCard)
                        .Take(requiredGroup.requiredCount)
                        .ToList();
                }
                else
                {
                    foundCards = availableIngredients
                        .Where(c => c.CardData.cardType == requiredGroup.cardType)
                        .Take(requiredGroup.requiredCount)
                        .ToList();
                }

                cardsToDestroy.AddRange(foundCards);

                foreach (var card in foundCards)
                {
                    availableIngredients.Remove(card);
                }
            }
        }

        // (�ָ�) ʹ�ô�������Ч���Ͷ���ص������߼�
        foreach (var cardToReturn in cardsToDestroy.Distinct().Reverse())
        {
            if (cardToReturn != null)
            {
                CardStacker parentStacker = cardToReturn.Stacker.Parent;
                if (parentStacker != null)
                {
                    parentStacker.SafelyRemoveChild(cardToReturn.Stacker);
                }

                if (EffectManager.Instance != null)
                {
                    EffectManager.Instance.PlayDissipateEffect(cardToReturn.transform.position);
                }

                cardToReturn.gameObject.SetActive(false);
                StartCoroutine(ReturnCardToPoolAfterDelay(cardToReturn, 1.0f));
            }
        }
        // --- �޸����� ---

        // ��������״̬
        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;
    }
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

            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }

    // (�ָ�) �¿��Ƶ����Ļ�������Э��
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
            float easedT = 1 - (1 - t) * (1 - t); // Ease-Out Quad ����Ч��

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

    // (�ָ�) �ӳٷ������Ƶ�����ص�Э��
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
}