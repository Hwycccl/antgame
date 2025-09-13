// �ļ�·��: Assets/script/CardsBasic/CardCombiner.cs
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
        // ������ںϳɣ����ҽ��������ϳɹ��򶼴��ڣ�����½���������ʾ
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
        // ֻ���ƶѵĸ������ܷ�����
        if (card.Stacker.Parent != null) return;
        // ������ںϳ��м������¿�����ȡ���ɵĺϳɣ����¼��
        if (isCombining) CancelCombination();

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();

        // �����ݿ��в���ƥ��ĺϳɹ���
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        // ����ҵ���ƥ��Ĺ����������ϳɹ���
        if (matchedRule != null)
        {
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    /// <summary>
    /// ִ�кϳɹ��̵�Э��
    /// </summary>
    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        // ֻ�е��ϳ�ʱ�����0��Ԥ�Ƽ�������ʱ���Ŵ�������ʾ������
        if (rule.time > 0 && combinationProgressPrefab != null)
        {
            GameObject progressBarInstance = Instantiate(combinationProgressPrefab, transform.position + progressBarOffset, Quaternion.identity, transform);
            currentProgressBar = progressBarInstance.GetComponent<CombinationProgressUI>();
        }

        // �ȴ��ϳ������ʱ��
        if (rule.time > 0)
        {
            yield return new WaitForSeconds(rule.time);
        }
        else
        {
            // ���ʱ��Ϊ0�������ٵȴ�һ֡����ȷ���߼����̵��ȶ�
            yield return null;
        }

        // �ϳ���ɣ����ٽ�����
        if (currentProgressBar != null)
        {
            Destroy(currentProgressBar.gameObject);
        }

        // ����ϳɽ���������¿���
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

        // ����ϳ����ģ�����ԭ�Ͽ���
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

        foreach (var cardToDestroy in cardsToDestroy.Distinct().Reverse())
        {
            if (cardToDestroy != null)
            {
                Destroy(cardToDestroy.gameObject);
            }
        }

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

    /// <summary>
    /// ��ȡ��ǰ�ϳɵ�ʣ��ʱ�䣨��CardDragger�ű����ã�
    /// </summary>
    /// <returns>ʣ�������</returns>
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