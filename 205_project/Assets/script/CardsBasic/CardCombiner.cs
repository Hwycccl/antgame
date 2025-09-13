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
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0); // 这个决定了卡牌最终弹出的位置

    // --- 新增变量 ---
    [Header("弹出动画设置")]
    [Tooltip("新卡牌弹出的动画时长")]
    [SerializeField] private float spawnAnimationDuration = 0.3f;
    // --- 新增结束 ---


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

        Debug.Log("合成完成！");

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
                            Debug.LogWarning("合成失败：蚁穴已满，无法产生新的蚂蚁！");
                            continue;
                        }
                    }

                    // --- 修改点 1: 在中心生成卡牌，并获取它的引用 ---
                    // 注意，我们现在在 rootPosition 生成它，而不是 rootPosition + spawnOffset
                    Card newCard = CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition);

                    // --- 修改点 2: 如果成功生成了卡牌，就为它启动弹出动画 ---
                    if (newCard != null)
                    {
                        Vector3 targetPosition = rootPosition + spawnOffset;
                        StartCoroutine(AnimateCardSpawn(newCard, targetPosition));
                    }

                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                }
            }
        }

        // (销毁卡牌的逻辑保持不变)
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

    // --- 新增方法: 卡牌弹出动画协程 ---
    // --- 修改后的方法: 卡牌弹出动画协程 (带有缓出效果) ---
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
            // --- 核心修改点 ---
            // 1. 计算原始的线性进度 (0 到 1)
            float t = elapsedTime / spawnAnimationDuration;

            // 2. 将线性进度通过缓动函数转换为曲线进度
            //    这个公式 (1 - (1 - t) * (1 - t)) 就是一个简单的 "Ease-Out Quad" 效果
            float easedT = 1 - (1 - t) * (1 - t);

            // 3. 使用新的曲线进度进行插值
            cardToAnimate.transform.position = Vector3.Lerp(startingPosition, targetPosition, easedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // --- 修改结束 ---

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
        // ... (此方法无变化) ...
        if (isCombining && currentCombinationRule != null)
        {
            float elapsedTime = Time.time - combinationStartTime;
            return Mathf.Max(0, currentCombinationRule.time - elapsedTime);
        }
        return 0;
    }

    public void CancelCombination()
    {
        // ... (此方法无变化) ...
        if (combinationCoroutine != null)
        {
            StopCoroutine(combinationCoroutine);
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }
}