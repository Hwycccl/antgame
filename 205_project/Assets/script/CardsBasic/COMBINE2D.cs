//
// COMBINE2D.cs 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class COMBINE2D : MonoBehaviour
{
    [Header("組合配方資料庫")]
    [SerializeField] private CardsCombination combinationData;

    // --- 修改點 開始 ---
    [Header("生成設置")]
    [Tooltip("新卡牌生成時，相對於原料卡牌的水平偏移量")]
    [SerializeField] private float spawnOffset = 2.0f;
    // --- 修改點 結束 ---

    private Coroutine combinationCoroutine;
    private float remainingTime = 0f;
    private COMBINE2D combinationPartner;
    private CardsCombinationRule currentCombination;

    private CardsBehaviour cardBehaviour;

    void Awake()
    {
        cardBehaviour = GetComponent<CardsBehaviour>();
    }

    // (TryToCombineWithNearbyCards, AttemptToStartCombination, StartCombination, CombinationTimer 函式保持不變)
    // ...

    private void ResolveCombination()
    {
        if (combinationPartner == null || gameObject.GetInstanceID() > combinationPartner.gameObject.GetInstanceID())
        {
            ResetCombinationState();
            return;
        }

        if (currentCombination == null) return;

        // --- 核心修改點 開始 ---
        // 在銷毀原料卡牌前，先記錄下它們的位置用於計算生成點
        Vector3 selfPosition = transform.position;
        // --- 核心修改點 結束 ---


        bool destroySelf = false;
        bool destroyPartner = false;
        var selfData = cardBehaviour.GetCardData();
        var partnerData = combinationPartner.cardBehaviour.GetCardData();

        foreach (var req in currentCombination.requiredCards)
        {
            if (req.destroyOnCombine)
            {
                if (req.specificCard != null)
                {
                    if (selfData == req.specificCard) destroySelf = true;
                    if (partnerData == req.specificCard) destroyPartner = true;
                }
                else
                {
                    if (selfData.cardType == req.cardType) destroySelf = true;
                    if (partnerData.cardType == req.cardType) destroyPartner = true;
                }
            }
        }

        // 生成結果卡牌
        foreach (var result in currentCombination.results)
        {
            if (Random.value <= result.probability)
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    // --- 核心修改點 開始 ---
                    // 1. 計算生成位置：在當前卡牌位置的基礎上，向左偏移
                    Vector3 spawnPosition = selfPosition + new Vector3(-spawnOffset, 0, 0);

                    // 2. 通知邏輯層
                    CardsManager.Instance.AddCardToLogic(result.resultCard);

                    // 3. 通知UI層，並傳入卡牌數據和精確的生成位置
                    HandUI.Instance.AddCardToView(result.resultCard, spawnPosition);
                    // --- 核心修改點 結束 ---
                }
            }
        }

        GameObject partnerObject = combinationPartner.gameObject;

        if (destroySelf)
        {
            CardsManager.Instance.RemoveCardFromLogic(selfData);
            Destroy(gameObject);
        }
        if (destroyPartner)
        {
            CardsManager.Instance.RemoveCardFromLogic(partnerData);
            Destroy(partnerObject);
        }

        if (!destroySelf) ResetCombinationState();
    }

    // (其他函式保持不變)
    // ...

    // --- 省略其他不變的函式以節省篇幅 ---
    #region Unchanged Methods 
    public bool TryToCombineWithNearbyCards()
    {
        if (IsInCombination()) return false;
        float detectionRadius = 1.5f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject) continue;
            var otherCombineScript = hitCollider.GetComponent<COMBINE2D>();
            if (otherCombineScript != null && !otherCombineScript.IsInCombination())
            {
                if (AttemptToStartCombination(otherCombineScript)) { return true; }
            }
        }
        return false;
    }
    private bool AttemptToStartCombination(COMBINE2D targetCombine)
    {
        if (combinationData == null) return false;
        var card1Data = cardBehaviour.GetCardData();
        var card2Data = targetCombine.cardBehaviour.GetCardData();
        if (card1Data == null || card2Data == null) return false;
        var inputCards = new List<CardsBasicData> { card1Data, card2Data };
        var combination = combinationData.GetCombination(inputCards);
        if (combination != null)
        {
            StartCombination(combination, targetCombine);
            targetCombine.StartCombination(combination, this);
            return true;
        }
        return false;
    }
    private void StartCombination(CardsCombinationRule combination, COMBINE2D partner)
    {
        currentCombination = combination;
        combinationPartner = partner;
        remainingTime = currentCombination.time;
        if (combinationCoroutine != null) { StopCoroutine(combinationCoroutine); }
        combinationCoroutine = StartCoroutine(CombinationTimer());
    }
    private IEnumerator CombinationTimer()
    {
        Debug.Log($"合成開始: {currentCombination.combinationName}, 需要 {remainingTime} 秒.");
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("合成完成!");
        ResolveCombination();
    }
    private void ResetCombinationState()
    {
        if (combinationCoroutine != null) StopCoroutine(combinationCoroutine);
        combinationCoroutine = null;
        remainingTime = 0f;
        combinationPartner = null;
        currentCombination = null;
    }
    public bool IsInCombination() => currentCombination != null;
    #endregion
}