// �����: CardCombiner.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCombiner : MonoBehaviour
{
    [Header("�M�ϔ�����")]
    [Tooltip("�������кϳ��䷽�� ScriptableObject")]
    [SerializeField] private CardsCombination combinationDatabase;
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0); // ��������˿������յ�����λ��

    // --- �������� ---
    [Header("������������")]
    [Tooltip("�¿��Ƶ����Ķ���ʱ��")]
    [SerializeField] private float spawnAnimationDuration = 0.3f;
    // --- �������� ---


    private Card card;
    private Coroutine combinationCoroutine;

    public bool isCombining { get; private set; } = false;
    private float combinationStartTime;
    private CardsCombinationRule currentCombinationRule;

    private void Awake()
    {
        card = GetComponent<Card>();
    }

    public void CheckForCombination()
    {
        if (card.Stacker.Parent != null) return;

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

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        yield return new WaitForSeconds(rule.time);

        Debug.Log("�ϳ���ɣ�");

        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCombinationEffect(transform.position);
        }

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

                    // --- �޸ĵ� 1: ���������ɿ��ƣ�����ȡ�������� ---
                    // ע�⣬���������� rootPosition �������������� rootPosition + spawnOffset
                    Card newCard = CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition);

                    // --- �޸ĵ� 2: ����ɹ������˿��ƣ���Ϊ�������������� ---
                    if (newCard != null)
                    {
                        Vector3 targetPosition = rootPosition + spawnOffset;
                        StartCoroutine(AnimateCardSpawn(newCard, targetPosition));
                    }

                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                }
            }
        }

        // (���ٿ��Ƶ��߼����ֲ���)
        List<Card> cardsToDestroy = new List<Card>();
        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                var matchingCards = ingredientCards
                    .Where(c => c.CardData == requiredGroup.specificCard || c.CardData.cardType == requiredGroup.cardType)
                    .Take(requiredGroup.requiredCount);
                cardsToDestroy.AddRange(matchingCards);
            }
        }

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

        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;
    }

    // --- ��������: ���Ƶ�������Э�� ---
    // --- �޸ĺ�ķ���: ���Ƶ�������Э�� (���л���Ч��) ---
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
            // --- �����޸ĵ� ---
            // 1. ����ԭʼ�����Խ��� (0 �� 1)
            float t = elapsedTime / spawnAnimationDuration;

            // 2. �����Խ���ͨ����������ת��Ϊ���߽���
            //    �����ʽ (1 - (1 - t) * (1 - t)) ����һ���򵥵� "Ease-Out Quad" Ч��
            float easedT = 1 - (1 - t) * (1 - t);

            // 3. ʹ���µ����߽��Ƚ��в�ֵ
            cardToAnimate.transform.position = Vector3.Lerp(startingPosition, targetPosition, easedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // --- �޸Ľ��� ---

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
        // ... (�˷����ޱ仯) ...
        if (isCombining && currentCombinationRule != null)
        {
            float elapsedTime = Time.time - combinationStartTime;
            return Mathf.Max(0, currentCombinationRule.time - elapsedTime);
        }
        return 0;
    }

    public void CancelCombination()
    {
        // ... (�˷����ޱ仯) ...
        if (combinationCoroutine != null)
        {
            StopCoroutine(combinationCoroutine);
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }
}