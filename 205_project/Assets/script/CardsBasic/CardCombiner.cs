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
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0); // 在左邊生成新卡

    private Card card;
    private Coroutine combinationCoroutine;

    private void Awake()
    {
        card = GetComponent<Card>();
    }

    // 由 CardStacker 在堆疊完成後呼叫
    public void CheckForCombination()
    {
        // 只有堆疊的根卡牌才負責檢查和執行合成
        if (card.Stacker.Parent != null) return;

        // 如果正在合成中，則不進行新的檢測
        if (combinationCoroutine != null) return;

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();

        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        if (matchedRule != null)
        {
            // 找到了配方，開始計時合成
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        Debug.Log($"找到組合: {rule.combinationName}，開始計時 {rule.time} 秒。");

        // 可以在這裡創建一個進度條UI
        // ...

        yield return new WaitForSeconds(rule.time);

        Debug.Log("合成完成！");

        // 1. 生成產物
        Vector3 rootPosition = transform.position;
        foreach (var result in rule.results)
        {
            if (Random.value <= result.probability) // 考慮機率
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition + spawnOffset);
                }
            }
        }

        // 2. 銷毀原料 (從子級開始銷毀，避免出錯)
        List<Card> cardsToDestroy = new List<Card>();
        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                // 找出需要被銷毀的卡牌實例
                var matchingCards = ingredientCards
                    .Where(c => c.CardData == requiredGroup.specificCard || c.CardData.cardType == requiredGroup.cardType)
                    .Take(requiredGroup.requiredCount);
                cardsToDestroy.AddRange(matchingCards);
            }
        }

        // 執行銷毀
        foreach (var cardToDestroy in cardsToDestroy.Distinct().Reverse())
        {
            Destroy(cardToDestroy.gameObject);
        }

        combinationCoroutine = null;
    }
}
