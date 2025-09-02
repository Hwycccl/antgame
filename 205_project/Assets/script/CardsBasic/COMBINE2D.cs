// COMBINE2D.cs (最終功能版)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(STACK2D))]
public class COMBINE2D : MonoBehaviour
{
    [Header("組合規則數據庫")]
    [Tooltip("請將定義了所有合成配方的 ScriptableObject 拖到此處")]
    [SerializeField] private CardsCombination combinationDatabase;

    private STACK2D stackScript;

    private void Awake()
    {
        stackScript = GetComponent<STACK2D>();
    }

    /// <summary>
    /// 嘗試對目標堆疊進行合成檢查 (由 CardsBehaviour 調用)
    /// </summary>
    /// <param name="targetStack">要進行合成檢查的目標堆疊的根卡牌</param>
    public void TryToCombineWithNearbyCards(STACK2D targetStack)
    {
        if (combinationDatabase == null)
        {
            Debug.LogError("組合規則數據庫 (Combination Database) 未設置！");
            return;
        }

        // 1. 獲取目標堆疊中所有的卡牌數據
        List<CardsBasicData> inputCardsData = targetStack.GetCardsDataInStack();

        // 2. 在數據庫中查找是否有匹配的組合規則
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputCardsData);

        // 3. 如果找到了匹配的規則，則執行合成
        if (matchedRule != null)
        {
            Debug.Log("成功匹配組合規則: " + matchedRule.combinationName);
            ExecuteCombination(targetStack, matchedRule);
        }
    }

    /// <summary>
    /// 執行合成過程：銷毀原料，生成產物
    /// </summary>
    private void ExecuteCombination(STACK2D rootStack, CardsCombinationRule rule)
    {
        // --- 處理原料 ---
        List<STACK2D> allStacksInGroup = new List<STACK2D>();
        CollectStacksRecursively(rootStack, allStacksInGroup);
        allStacksInGroup.Reverse(); // 從子級開始處理，避免父級先被銷毀

        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                int countToDestroy = requiredGroup.requiredCount;
                // 從後往前遍歷，安全地刪除
                for (int i = allStacksInGroup.Count - 1; i >= 0; i--)
                {
                    if (countToDestroy <= 0) break;

                    STACK2D currentStack = allStacksInGroup[i];
                    CardsBasicData cardData = currentStack.GetComponent<CardsBehaviour>().GetCardData();

                    bool matchesSpecific = requiredGroup.specificCard != null && requiredGroup.specificCard == cardData;
                    bool matchesType = requiredGroup.specificCard == null && requiredGroup.cardType == cardData.cardType;

                    if (matchesSpecific || matchesType)
                    {
                        // 從邏輯管理器和場景中移除卡牌
                        CardsManager.Instance.RemoveCardFromLogic(cardData);
                        Destroy(currentStack.gameObject);
                        countToDestroy--;
                    }
                }
            }
        }

        // --- 處理產物 ---
        Vector3 spawnPosition = rootStack.transform.position; // 在根卡牌的位置生成
        foreach (var resultCardInfo in rule.results)
        {
            // 考慮生成機率
            if (Random.value <= resultCardInfo.probability)
            {
                for (int i = 0; i < resultCardInfo.quantity; i++)
                {
                    // 稍微錯開位置，避免完全重疊
                    Vector3 offset = new Vector3(Random.Range(-0.1f, 0.1f), 0, 0);

                    // 通知邏輯和UI生成新卡
                    CardsManager.Instance.AddCardToLogic(resultCardInfo.resultCard);
                    HandUI.Instance.AddCardToView(resultCardInfo.resultCard, spawnPosition + offset);
                }
            }
        }
    }

    // 輔助方法：遞迴收集堆疊中的所有 STACK2D 組件
    private void CollectStacksRecursively(STACK2D stack, List<STACK2D> stackList)
    {
        if (stack == null || stackList.Contains(stack)) return;

        stackList.Add(stack);
        foreach (var child in stack.ChildStacks)
        {
            CollectStacksRecursively(child, stackList);
        }
    }
}