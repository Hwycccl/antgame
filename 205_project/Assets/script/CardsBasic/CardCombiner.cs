// 文件路径: Assets/script/CardsBasic/CardCombiner.cs (已修复)
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

    [Header("弹出动画设置")]
    [Tooltip("新卡牌弹出的动画时长")]
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
        // 核心修复 #1: 如果正在合成，先取消旧的
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

    // --- 核心修复 #2: 重新添加回健壮的 CancelCombination 公共方法 ---
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

            // 重置所有状态
            combinationCoroutine = null;
            isCombining = false;
            currentCombinationRule = null;
        }
    }

    // 文件路径: Assets/script/CardsBasic/CardCombiner.cs (已修复)

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

        // --- 阶段1: 可中断的计时阶段 ---
        if (rule.time > 0)
        {
            float timer = rule.time;
            while (timer > 0)
            {
                // 每一帧都检查卡牌堆叠是否被玩家破坏
                if (!IsStackIntact(ingredientCards))
                {
                    Debug.Log("合成过程被玩家拖动卡牌中断。");
                    CancelCombination(); // 安全地取消合成
                    yield break;         // 退出协程
                }
                timer -= Time.deltaTime;
                yield return null; // 等待下一帧
            }
        }

        // 在执行前做最后一次检查，防止在最后一帧被拖走
        if (!IsStackIntact(ingredientCards))
        {
            Debug.Log("合成在执行前一刻被中断。");
            CancelCombination();
            yield break;
        }

        // --- 阶段2: 不可中断的执行阶段 ---
        // 锁定所有参与的卡牌，防止在生成和销毁动画期间出现BUG
        foreach (var ingredient in ingredientCards)
        {
            if (ingredient != null && ingredient.Dragger != null)
            {
                ingredient.Dragger.enabled = false;
            }
        }

        // 清理进度条
        if (currentProgressBar != null)
        {
            Destroy(currentProgressBar.gameObject);
        }

        // 播放特效
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCombinationEffect(transform.position);
        }

        // --- 开始生成新卡牌 ---
        Vector3 rootPosition = transform.position;
        foreach (var result in rule.results)
        {
            if (Random.value <= result.probability)
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    if (result.resultCard.cardType == CardsBasicData.CardType.Ant && PopulationManager.Instance != null && PopulationManager.Instance.IsPopulationFull())
                    {
                        Debug.LogWarning("合成失败：蚁穴已满，无法产生新的蚂蚁！");
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

        // --- 开始销毁旧卡牌 ---
        List<Card> cardsToDestroy = new List<Card>();
        List<Card> availableIngredients = new List<Card>(ingredientCards); // 这将是那些不被销毁的卡牌
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
                    availableIngredients.Remove(card); // 从剩余列表中移除
                }
            }
        }

        // ... (销毁逻辑和对象池回收，这部分保持不变) ...
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


        // --- 重置状态并解锁剩余卡牌 ---
        combinationCoroutine = null;
        isCombining = false;
        currentCombinationRule = null;

        foreach (var remainingCard in availableIngredients)
        {
            if (remainingCard != null && remainingCard.gameObject.activeInHierarchy)
            {
                // 重新启用未被销毁卡牌的拖动功能
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
    /// 辅助方法：检查初始的合成原料是否都还在当前的堆叠中
    /// </summary>
    private bool IsStackIntact(List<Card> originalIngredients)
    {
        List<Card> currentStack = card.Stacker.GetCardsInStack();
        // 检查数量是否一致，以及所有原始卡牌是否都还在
        if (currentStack.Count != originalIngredients.Count) return false;

        // Linq.All确保originalIngredients中的每一个元素都在currentStack中
        return originalIngredients.All(c => currentStack.Contains(c));
    }
}