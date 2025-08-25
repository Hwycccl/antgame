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
    private bool isPaused = false;
    private COMBINE2D combinationPartner;
    private CardsCombinationRule currentCombination;

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
            var inputCards = new List<CardsBasicData> { card1Data, card2Data };
            var combination = combinationData.GetCombination(inputCards);

            if (combination != null)
            {
                SetupCombination(combination, targetCombine);
                targetCombine.SetupCombination(combination, this);

                if (isInitiator)
                {
                    var timerCoroutine = StartCoroutine(CombinationTimer());
                    combinationCoroutine = timerCoroutine;
                    targetCombine.combinationCoroutine = timerCoroutine;
                }
                return true;
            }
        }
        return false;
    }

    private void SetupCombination(CardsCombinationRule combination, COMBINE2D partner)
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

        var inputCards = new List<CardsBasicData> {
            GetComponent<CardsBehaviour>().GetCardData(),
            combinationPartner.GetComponent<CardsBehaviour>().GetCardData()
        };

        bool selfSurvived = true;
        bool partnerSurvived = true;
        var partnerObject = combinationPartner.gameObject;

        foreach (var req in currentCombination.requiredCards)
        {
            if (req.specificCard != null)
            {
                if (inputCards.Contains(req.specificCard) && req.destroyOnCombine)
                {
                    if (GetComponent<CardsBehaviour>().GetCardData() == req.specificCard) selfSurvived = false;
                    if (combinationPartner.GetComponent<CardsBehaviour>().GetCardData() == req.specificCard) partnerSurvived = false;
                }
            }
            else
            {
                if (inputCards.Any(c => c.cardType == req.cardType) && req.destroyOnCombine)
                {
                    if (GetComponent<CardsBehaviour>().GetCardData().cardType == req.cardType) selfSurvived = false;
                    if (combinationPartner.GetComponent<CardsBehaviour>().GetCardData().cardType == req.cardType) partnerSurvived = false;
                }
            }
        }

        if (!selfSurvived) Destroy(gameObject);
        if (!partnerSurvived) Destroy(partnerObject);

        if (selfSurvived) ResetCombinationState(false);
        if (partnerSurvived && partnerObject != null)
            partnerObject.GetComponent<COMBINE2D>().ResetCombinationState(false);
    }

    private IEnumerator GenerateCardAfterDelay(CardsCombinationRule combination)
    {
        yield return null;

        if (combination.results.Count > 0)
        {
            foreach (var result in combination.results)
            {
                if (Random.value <= result.probability)
                {
                    for (int i = 0; i < result.quantity; i++)
                        HandUI.Instance.AddCardToHand(result.resultCard);
                }
            }
        }
    }

    public void PauseCombination()
    {
        if (combinationPartner != null)
        {
            isPaused = true;
            combinationPartner.isPaused = true;
            Debug.Log("计时暂停。");
        }
    }

    public void ResumeCombination()
    {
        if (combinationPartner != null && isPaused)
        {
            isPaused = false;
            combinationPartner.isPaused = false;
            Debug.Log("计时恢复。");
        }
    }

    public void CancelCombination()
    {
        if (combinationPartner != null)
            combinationPartner.ResetCombinationState(false);

        ResetCombinationState(true);
        Debug.Log("组合已取消。");
    }

    private void ResetCombinationState(bool notifyPartner)
    {
        if (combinationCoroutine != null) StopCoroutine(combinationCoroutine);

        if (notifyPartner && combinationPartner != null && combinationPartner.combinationPartner == this)
            combinationPartner.ResetCombinationState(false);

        combinationCoroutine = null;
        remainingTime = 0f;
        isPaused = false;
        combinationPartner = null;
        currentCombination = null;

        if (hoverDragScript) hoverDragScript.enabled = true;
        if (hoverDragScript) hoverDragScript.ResetSortingOrder();
    }

    public bool IsInCombination() => combinationPartner != null;

    public COMBINE2D GetPartner() => combinationPartner;
}
