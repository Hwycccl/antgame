// COMBINE2D.cs
using UnityEngine;
using System.Collections;

public class COMBINE2D : MonoBehaviour
{
    [Header("组合配方")]
    [SerializeField] private CardCombinationsData combinationData;

    private Coroutine combinationCoroutine;
    private float remainingTime = 0f;
    private bool isPaused = false;
    private COMBINE2D combinationPartner;
    private CardCombination currentCombination;

    private HoverDrag2D hoverDragScript;

    void Start()
    {
        hoverDragScript = GetComponent<HoverDrag2D>();
    }

    public bool AttemptToStartCombination(COMBINE2D targetCombine, bool isInitiator)
    {
        if (combinationData == null || combinationPartner != null || targetCombine.combinationPartner != null) return false;

        var thisCardBeh = GetComponent<CardsBehaviour>();
        var targetCardBeh = targetCombine.GetComponent<CardsBehaviour>();

        if (thisCardBeh != null && targetCardBeh != null)
        {
            var card1Data = thisCardBeh.GetCardData();
            var card2Data = targetCardBeh.GetCardData();

            var combination = combinationData.GetCombination(card1Data, card2Data);
            if (combination != null)
            {
                SetupCombination(combination, targetCombine);
                targetCombine.SetupCombination(combination, this);

                if (isInitiator)
                {
                    // 双方都保存对协程的引用，以便都能取消它
                    var timerCoroutine = StartCoroutine(CombinationTimer());
                    combinationCoroutine = timerCoroutine;
                    targetCombine.combinationCoroutine = timerCoroutine;
                }
                return true;
            }
        }
        return false;
    }

    private void SetupCombination(CardCombination combination, COMBINE2D partner)
    {
        currentCombination = combination;
        combinationPartner = partner;
        remainingTime = currentCombination.time;
    }

    private IEnumerator CombinationTimer()
    {
        Debug.Log("组合计时开始... 剩余 " + remainingTime + " 秒");
        while (remainingTime > 0)
        {
            if (!isPaused)
            {
                remainingTime -= Time.deltaTime;
                if (combinationPartner != null) combinationPartner.remainingTime = remainingTime;
            }
            yield return null;
        }

        Debug.Log("计时结束！");
        ResolveCombination();
    }

    private void ResolveCombination()
    {
        if (currentCombination == null || combinationPartner == null) return;

        HandUI.Instance.StartCoroutine(GenerateCardAfterDelay(currentCombination));

        var cardData1 = GetComponent<CardsBehaviour>().GetCardData();
        var cardData2 = combinationPartner.GetComponent<CardsBehaviour>().GetCardData();
        var rule1 = currentCombination.requiredCards[0];
        var rule2 = currentCombination.requiredCards[1];
        bool card1MatchesRule1 = (cardData1 == rule1.cardData && cardData2 == rule2.cardData);

        bool selfSurvived = true;
        bool partnerSurvived = true;
        var partnerObject = combinationPartner.gameObject; // 提前获取引用

        if (card1MatchesRule1)
        {
            if (rule1.destroyOnCombine) selfSurvived = false;
            if (rule2.destroyOnCombine) partnerSurvived = false;
        }
        else
        {
            if (rule2.destroyOnCombine) selfSurvived = false;
            if (rule1.destroyOnCombine) partnerSurvived = false;
        }

        if (!selfSurvived) Destroy(gameObject);
        if (!partnerSurvived) Destroy(partnerObject);

        // **核心修正**: 在流程的最后，通知所有幸存者重置状态
        if (selfSurvived)
        {
            ResetCombinationState(false); // 重置自己
        }
        if (partnerSurvived)
        {
            // 使用我们提前保存的引用
            if (partnerObject != null)
            {
                partnerObject.GetComponent<COMBINE2D>().ResetCombinationState(false); // 重置伙伴
            }
        }
    }

    private IEnumerator GenerateCardAfterDelay(CardCombination combination)
    {
        yield return null;

        if (combination.resultingCards.Count > 0)
        {
            int randomIndex = Random.Range(0, combination.resultingCards.Count);
            CardsBasicData resultingCardData = combination.resultingCards[randomIndex];
            HandUI.Instance.AddCardToHand(resultingCardData);
        }
    }

    public void PauseCombination()
    {
        if (combinationPartner != null)
        {
            isPaused = true;
            if (combinationPartner != null) combinationPartner.isPaused = true;
            Debug.Log("计时暂停。");
        }
    }

    public void ResumeCombination()
    {
        if (combinationPartner != null && isPaused)
        {
            isPaused = false;
            if (combinationPartner != null) combinationPartner.isPaused = false;
            Debug.Log("计时恢复。");
        }
    }

    public void CancelCombination()
    {
        if (combinationPartner != null)
        {
            combinationPartner.ResetCombinationState(false);
        }
        ResetCombinationState(true);
        Debug.Log("组合已取消。");
    }

    private void ResetCombinationState(bool notifyPartner)
    {
        if (combinationCoroutine != null) StopCoroutine(combinationCoroutine);

        if (notifyPartner && combinationPartner != null && combinationPartner.combinationPartner == this)
        {
            combinationPartner.ResetCombinationState(false);
        }

        combinationCoroutine = null;
        remainingTime = 0f;
        isPaused = false;
        combinationPartner = null;
        currentCombination = null;

        if (hoverDragScript) hoverDragScript.enabled = true;
        if (hoverDragScript) hoverDragScript.ResetSortingOrder();
    }

    public bool IsInCombination()
    {
        return combinationPartner != null;
    }

    public COMBINE2D GetPartner()
    {
        return combinationPartner;
    }
}