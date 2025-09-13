// 文件路径: Assets/script/CardsBasic/CardCombiner.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCombiner : MonoBehaviour
{
    [Header("組合數據庫")]
    [Tooltip("包含所有合成配方的 ScriptableObject")]
    [SerializeField] private CardsCombination combinationDatabase;
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0);

    [Header("UI设置")]
    [Tooltip("将你的进度条UI预制体拖到这里")]
    [SerializeField] private GameObject combinationProgressPrefab;
    [Tooltip("进度条相对于卡牌根部的位置偏移")]
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
        // 如果正在合成，并且进度条、合成规则都存在，则更新进度条的显示
        if (isCombining && currentProgressBar != null && currentCombinationRule != null && currentCombinationRule.time > 0)
        {
            float elapsedTime = Time.time - combinationStartTime;
            float progress = elapsedTime / currentCombinationRule.time;
            currentProgressBar.UpdateProgress(progress);
        }
    }

    /// <summary>
    /// 检查当前牌堆是否满足合成条件
    /// </summary>
    public void CheckForCombination()
    {
        // 只有牌堆的根卡才能发起检查
        if (card.Stacker.Parent != null) return;
        // 如果正在合成中加入了新卡，则取消旧的合成，重新检查
        if (isCombining) CancelCombination();

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();

        // 从数据库中查找匹配的合成规则
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        // 如果找到了匹配的规则，则启动合成过程
        if (matchedRule != null)
        {
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    /// <summary>
    /// 执行合成过程的协程
    /// </summary>
    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        // 只有当合成时间大于0且预制件已设置时，才创建并显示进度条
        if (rule.time > 0 && combinationProgressPrefab != null)
        {
            GameObject progressBarInstance = Instantiate(combinationProgressPrefab, transform.position + progressBarOffset, Quaternion.identity, transform);
            currentProgressBar = progressBarInstance.GetComponent<CombinationProgressUI>();
        }

        // 等待合成所需的时间
        if (rule.time > 0)
        {
            yield return new WaitForSeconds(rule.time);
        }
        else
        {
            // 如果时间为0，则至少等待一帧，以确保逻辑流程的稳定
            yield return null;
        }

        // 合成完成，销毁进度条
        if (currentProgressBar != null)
        {
            Destroy(currentProgressBar.gameObject);
        }

        // 处理合成结果：生成新卡牌
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
                            Debug.LogWarning("合成失败：蚁穴已满，无法产生新的蚂蚁！");
                            continue;
                        }
                    }
                    CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition + spawnOffset);
                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                }
            }
        }

        // 处理合成消耗：销毁原料卡牌
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

        // 重置所有状态
        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;
    }

    /// <summary>
    /// 从外部取消当前的合成过程
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
    /// 获取当前合成的剩余时间（给CardDragger脚本调用）
    /// </summary>
    /// <returns>剩余的秒数</returns>
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