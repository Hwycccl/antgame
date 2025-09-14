// 文件路径: Assets/script/CardsBasic/CardCombiner.cs (最终合并版)

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
        // (保留) 更新进度条的显示逻辑
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

        // (保留) 创建并显示进度条
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

        // (保留) 销毁进度条
        if (currentProgressBar != null)
        {
            Destroy(currentProgressBar.gameObject);
        }

        // (恢复) 播放合成粒子效果
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCombinationEffect(transform.position);
        }

        // --- 处理合成结果 (您修改后的版本，保持不变) ---
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

        // --- 核心修复：在这里添加回缺失的卡牌销毁逻辑 ---

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

        // (恢复) 使用带有粒子效果和对象池的销毁逻辑
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
        // --- 修复结束 ---

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

    // (恢复) 新卡牌弹出的缓动动画协程
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
            float easedT = 1 - (1 - t) * (1 - t); // Ease-Out Quad 缓动效果

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

    // (恢复) 延迟返还卡牌到对象池的协程
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