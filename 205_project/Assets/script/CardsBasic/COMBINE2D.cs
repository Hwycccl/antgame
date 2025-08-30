//COMBINE2D.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class COMBINE2D : MonoBehaviour
{
    [Header("组合配方数据库")]
    [SerializeField] private CardsCombination combinationData;

    private Coroutine combinationCoroutine;
    private float remainingTime = 0f;
    private COMBINE2D combinationPartner;
    private CardsCombinationRule currentCombination;

    private CardsBehaviour cardBehaviour;

    void Awake()
    {
        cardBehaviour = GetComponent<CardsBehaviour>();
    }

    /// <summary>
    /// 尝试与附近的卡牌进行合成，这是合成的发起点
    /// </summary>
    /// <returns>如果成功找到合成对象并开始合成，则返回true</returns>
    public bool TryToCombineWithNearbyCards()
    {
        // 如果已经在合成中，则直接返回
        if (IsInCombination()) return false;

        // 设置一个检测半径来寻找附近的卡牌
        float detectionRadius = 1.5f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // 排除自己
            if (hitCollider.gameObject == gameObject) continue;

            var otherCombineScript = hitCollider.GetComponent<COMBINE2D>();
            // 检查对方是否存在且没有在合成中
            if (otherCombineScript != null && !otherCombineScript.IsInCombination())
            {
                // 尝试与这张卡牌开始合成
                if (AttemptToStartCombination(otherCombineScript))
                {
                    return true; // 成功找到并开始合成
                }
            }
        }

        return false; // 没有找到可以合成的对象
    }

    /// <summary>
    /// 尝试与一个具体的目标卡牌开始合成流程
    /// </summary>
    private bool AttemptToStartCombination(COMBINE2D targetCombine)
    {
        if (combinationData == null) return false;

        var card1Data = cardBehaviour.GetCardData();
        var card2Data = targetCombine.cardBehaviour.GetCardData();

        if (card1Data == null || card2Data == null) return false;

        var inputCards = new List<CardsBasicData> { card1Data, card2Data };
        var combination = combinationData.GetCombination(inputCards);

        // 如果找到了匹配的合成规则
        if (combination != null)
        {
            // 在自己和对方身上都启动合成流程
            StartCombination(combination, targetCombine);
            targetCombine.StartCombination(combination, this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 设置合成状态并启动计时器
    /// </summary>
    private void StartCombination(CardsCombinationRule combination, COMBINE2D partner)
    {
        currentCombination = combination;
        combinationPartner = partner;
        remainingTime = currentCombination.time;

        // 停止可能存在的旧计时器
        if (combinationCoroutine != null)
        {
            StopCoroutine(combinationCoroutine);
        }
        combinationCoroutine = StartCoroutine(CombinationTimer());
    }

    /// <summary>
    /// 合成倒计时协程
    /// </summary>
    private IEnumerator CombinationTimer()
    {
        Debug.Log($"合成开始: {currentCombination.combinationName}, 需要 {remainingTime} 秒.");
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            // 可以在这里更新UI来显示剩余时间
            yield return null;
        }

        Debug.Log("合成完成!");
        ResolveCombination();
    }

    /// <summary>
    /// 处理合成结果，销毁原料并生成新卡牌
    /// </summary>
    private void ResolveCombination()
    {
        if (currentCombination == null || combinationPartner == null) return;

        // 标记需要销毁的卡牌
        bool destroySelf = false;
        bool destroyPartner = false;

        var selfData = cardBehaviour.GetCardData();
        var partnerData = combinationPartner.cardBehaviour.GetCardData();

        foreach (var req in currentCombination.requiredCards)
        {
            if (req.destroyOnCombine)
            {
                if (req.specificCard != null) // 按特定卡牌匹配
                {
                    if (selfData == req.specificCard) destroySelf = true;
                    if (partnerData == req.specificCard) destroyPartner = true;
                }
                else // 按卡牌类型匹配
                {
                    if (selfData.cardType == req.cardType) destroySelf = true;
                    if (partnerData.cardType == req.cardType) destroyPartner = true;
                }
            }
        }

        // 生成结果卡牌
        foreach (var result in currentCombination.results)
        {
            if (Random.value <= result.probability) // 按概率生成
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    HandUI.Instance.AddCardToHand(result.resultCard);
                }
            }
        }

        // 获取伙伴的游戏对象，以防它先被销毁
        GameObject partnerObject = combinationPartner.gameObject;

        // 销毁卡牌
        if (destroySelf)
        {
            Destroy(gameObject);
        }
        if (destroyPartner)
        {
            Destroy(partnerObject);
        }

        // 重置状态
        if (!destroySelf) ResetCombinationState();
        if (!destroyPartner && partnerObject != null)
            partnerObject.GetComponent<COMBINE2D>().ResetCombinationState();

    }

    /// <summary>
    /// 重置合成状态
    /// </summary>
    private void ResetCombinationState()
    {
        if (combinationCoroutine != null) StopCoroutine(combinationCoroutine);

        combinationCoroutine = null;
        remainingTime = 0f;
        combinationPartner = null;
        currentCombination = null;
    }

    /// <summary>
    /// 检查当前卡牌是否正在合成中
    /// </summary>
    public bool IsInCombination() => currentCombination != null;
}
