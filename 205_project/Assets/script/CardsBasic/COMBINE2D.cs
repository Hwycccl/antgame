// COMBINE2D.cs
using UnityEngine;
using System.Collections;

public class COMBINE2D : MonoBehaviour
{
    [Header("����䷽")]
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
                    // ˫���������Э�̵����ã��Ա㶼��ȡ����
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
        Debug.Log("��ϼ�ʱ��ʼ... ʣ�� " + remainingTime + " ��");
        while (remainingTime > 0)
        {
            if (!isPaused)
            {
                remainingTime -= Time.deltaTime;
                if (combinationPartner != null) combinationPartner.remainingTime = remainingTime;
            }
            yield return null;
        }

        Debug.Log("��ʱ������");
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
        var partnerObject = combinationPartner.gameObject; // ��ǰ��ȡ����

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

        // **��������**: �����̵����֪ͨ�����Ҵ�������״̬
        if (selfSurvived)
        {
            ResetCombinationState(false); // �����Լ�
        }
        if (partnerSurvived)
        {
            // ʹ��������ǰ���������
            if (partnerObject != null)
            {
                partnerObject.GetComponent<COMBINE2D>().ResetCombinationState(false); // ���û��
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
            Debug.Log("��ʱ��ͣ��");
        }
    }

    public void ResumeCombination()
    {
        if (combinationPartner != null && isPaused)
        {
            isPaused = false;
            if (combinationPartner != null) combinationPartner.isPaused = false;
            Debug.Log("��ʱ�ָ���");
        }
    }

    public void CancelCombination()
    {
        if (combinationPartner != null)
        {
            combinationPartner.ResetCombinationState(false);
        }
        ResetCombinationState(true);
        Debug.Log("�����ȡ����");
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