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
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0);

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
        // ������ȷ����ֻ���ƶѵĸ������ܷ���ϳɣ�������ȷ�ģ�Ӧ������
        if (card.Stacker.Parent != null) return;

        // --- �����޸ĵ� ��ʼ ---
        // ��鵱ǰ�Ƿ��Ѿ���һ���ϳ����ڽ�����
        if (isCombining)
        {
            // ����ǣ���ȡ����ǰ�ĺϳɣ�Ϊ���������¼����׼��
            CancelCombination();
            Debug.Log("A new card was added to the stack. The current combination has been cancelled and will be re-evaluated.");
        }
        // --- �����޸ĵ� ���� ---

        // �����߼����ֲ��䣬�����ð������¿��Ƶ������ƶ�ȥѰ���䷽
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
        Debug.Log($"�ҵ��M��: {rule.combinationName}���_ʼӋ�r {rule.time} �롣");
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        yield return new WaitForSeconds(rule.time);

        Debug.Log("�ϳ���ɣ�");

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
                    CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition + spawnOffset);
                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                }
            }
        }

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

        // �����������������ƣ�����������
        foreach (var cardToReturn in cardsToDestroy.Distinct().Reverse())
        {
            if (cardToReturn != null)
            {
                CardPool.Instance.Return(cardToReturn); // ʹ��Return����Destroy
            }
        }


        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;
        // ��ѡ���Ƽ����ںϳɺ��ø����Ƹ���һ���Ӿ���ȷ���㼶����ȷ
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
    /// ���ⲿֹͣ��ǰ�ĺϳɹ��̡�
    /// </summary>
    public void CancelCombination()
    {
        // ��鵱ǰ�Ƿ������ڽ��еĺϳ�Э��
        if (combinationCoroutine != null)
        {
            Debug.Log($"�ϳɹ��� '{currentCombinationRule.combinationName}' �ѱ�ȡ����");

            // ֹͣЭ��
            StopCoroutine(combinationCoroutine);

            // ��������״̬�������ص�δ�ϳ�״̬
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }
}