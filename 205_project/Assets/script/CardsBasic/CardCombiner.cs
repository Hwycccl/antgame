// 放置於: CardCombiner.cs
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
        // 这个检查确保了只有牌堆的根卡才能发起合成，这是正确的，应当保留
        if (card.Stacker.Parent != null) return;

        // --- 核心修改点 开始 ---
        // 检查当前是否已经有一个合成正在进行中
        if (isCombining)
        {
            // 如果是，则取消当前的合成，为接下来的新检测做准备
            CancelCombination();
            Debug.Log("A new card was added to the stack. The current combination has been cancelled and will be re-evaluated.");
        }
        // --- 核心修改点 结束 ---

        // 后续逻辑保持不变，它会用包含了新卡牌的完整牌堆去寻找配方
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
        Debug.Log($"找到組合: {rule.combinationName}，開始計時 {rule.time} 秒。");
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        yield return new WaitForSeconds(rule.time);

        Debug.Log("合成完成！");

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

        // 遍历并“返还”卡牌，而不是销毁
        foreach (var cardToReturn in cardsToDestroy.Distinct().Reverse())
        {
            if (cardToReturn != null)
            {
                CardPool.Instance.Return(cardToReturn); // 使用Return代替Destroy
            }
        }


        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;
        // 可选但推荐：在合成后，让根卡牌更新一下视觉，确保层级等正确
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
    /// 从外部停止当前的合成过程。
    /// </summary>
    public void CancelCombination()
    {
        // 检查当前是否有正在进行的合成协程
        if (combinationCoroutine != null)
        {
            Debug.Log($"合成过程 '{currentCombinationRule.combinationName}' 已被取消。");

            // 停止协程
            StopCoroutine(combinationCoroutine);

            // 重置所有状态变量，回到未合成状态
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }
}