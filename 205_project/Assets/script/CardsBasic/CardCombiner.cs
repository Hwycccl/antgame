// 文件路径: Assets/script/CardsBasic/CardCombiner.cs (最终修复版 - 正确处理父子关系)
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
        if (card.Stacker.Parent != null || isCombining) return;

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        if (matchedRule != null)
        {
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

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

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> initialIngredientCards)
    {
        isCombining = true;
        combinationStartTime = Time.time;
        currentCombinationRule = rule;

        // --- 正常合成流程 ---
        if (rule.time > 0 && combinationProgressPrefab != null)
        {
            GameObject progressBarInstance = Instantiate(combinationProgressPrefab, transform.position + progressBarOffset, Quaternion.identity, transform);
            currentProgressBar = progressBarInstance.GetComponent<CombinationProgressUI>();
        }
        if (rule.time > 0) yield return new WaitForSeconds(rule.time);
        if (currentProgressBar != null) Destroy(currentProgressBar.gameObject);
        if (EffectManager.Instance != null) EffectManager.Instance.PlayCombinationEffect(transform.position);

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
                    if (newCard != null) StartCoroutine(AnimateCardSpawn(newCard, rootPosition + spawnOffset));
                    UnlockedCardsManager.UnlockCard(result.resultCard.cardName);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        yield return new WaitForSeconds(spawnAnimationDuration + 0.1f);

        // ====================【修复逻辑开始】====================

        // 1. 识别要销毁的卡牌和幸存的卡牌
        var cardsToDestroy = new HashSet<Card>();
        var tempIngredients = new List<Card>(initialIngredientCards);
        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                var foundCards = tempIngredients
                    .Where(c => requiredGroup.specificCard != null ? c.CardData == requiredGroup.specificCard : c.CardData.cardType == requiredGroup.cardType)
                    .Take(requiredGroup.requiredCount).ToList();

                foreach (var foundCard in foundCards)
                {
                    cardsToDestroy.Add(foundCard);
                    tempIngredients.Remove(foundCard);
                }
            }
        }
        var survivingCards = initialIngredientCards.Where(c => !cardsToDestroy.Contains(c)).ToList();

        // 2.【关键】处理孤儿：让幸存的子卡牌主动与即将被销毁的父卡牌断开连接
        foreach (var cardToDestroy in cardsToDestroy)
        {
            var childrenCopy = cardToDestroy.Stacker.GetChildren();
            foreach (var childStacker in childrenCopy)
            {
                Card childCard = childStacker.GetComponent<Card>();
                if (childCard != null && !cardsToDestroy.Contains(childCard))
                {
                    childStacker.DetachFromParent(); // 这个方法会清空子卡牌的Parent引用，使其成为新的根
                }
            }
        }

        // 3. 销毁卡牌
        foreach (var cardToReturn in cardsToDestroy)
        {
            if (cardToReturn != null)
            {
                if (EffectManager.Instance != null) EffectManager.Instance.PlayDissipateEffect(cardToReturn.transform.position);
                StartCoroutine(ReturnCardToPoolAfterDelay(cardToReturn, 0.5f));
            }
        }

        // 4. 重置当前合成器的状态
        isCombining = false;
        combinationCoroutine = null;
        currentCombinationRule = null;

        yield return new WaitForSeconds(0.1f); // 等待一小会儿，确保销毁操作已开始

        // 5.【关键】状态重建与重新检查
        if (survivingCards.Count > 0)
        {
            // 找到所有幸存者中，现在是根卡牌的那些（即成为了新的独立牌堆）
            var newRoots = survivingCards.Where(c => c != null && c.gameObject.activeInHierarchy && c.Stacker.Parent == null).ToList();

            Debug.Log($"合成完毕，找到 {newRoots.Count} 个新的独立牌堆。");

            // 如果幸存者形成了多个独立的牌堆，将它们重新组合起来
            if (newRoots.Count > 1)
            {
                // 找到最底部的牌作为地基
                var baseCard = newRoots.OrderBy(c => c.transform.position.y).First();
                // 其他所有新的根都堆叠到这个地基上
                foreach (var root in newRoots)
                {
                    if (root != baseCard)
                    {
                        root.Stacker.StackOn(baseCard.Stacker.GetTopmostCardInStack());
                        yield return new WaitForSeconds(0.2f); // 等待堆叠动画完成
                    }
                }
            }

            // 最后，对最终形成的那个牌堆的根，发起一次新的合成检查
            var finalRoot = survivingCards.FirstOrDefault(c => c != null && c.gameObject.activeInHierarchy)?.Stacker.GetRoot();
            if (finalRoot != null)
            {
                Debug.Log($"对新的根 [{finalRoot.name}] 重新检查合成。");
                finalRoot.GetComponent<Card>().Combiner.CheckForCombination();
            }
        }

        // ====================【修复逻辑结束】====================
    }

    private IEnumerator AnimateCardSpawn(Card cardToAnimate, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = cardToAnimate.transform.position;
        if (cardToAnimate.Dragger != null) cardToAnimate.Dragger.enabled = false;

        while (elapsedTime < spawnAnimationDuration)
        {
            float t = elapsedTime / spawnAnimationDuration;
            float easedT = 1 - (1 - t) * (1 - t);
            cardToAnimate.transform.position = Vector3.Lerp(startingPosition, targetPosition, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardToAnimate.transform.position = targetPosition;
        if (cardToAnimate.Dragger != null) cardToAnimate.Dragger.enabled = true;
    }

    private IEnumerator ReturnCardToPoolAfterDelay(Card cardToReturn, float delay)
    {
        if (cardToReturn == null) yield break;

        cardToReturn.gameObject.SetActive(false);
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