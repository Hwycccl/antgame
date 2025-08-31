//
// COMBINE2D.cs 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class COMBINE2D : MonoBehaviour
{
    [Header("�M���䷽�Y�ώ�")]
    [SerializeField] private CardsCombination combinationData;

    // --- �޸��c �_ʼ ---
    [Header("�����O��")]
    [Tooltip("�¿������ɕr�������ԭ�Ͽ��Ƶ�ˮƽƫ����")]
    [SerializeField] private float spawnOffset = 2.0f;
    // --- �޸��c �Y�� ---

    private Coroutine combinationCoroutine;
    private float remainingTime = 0f;
    private COMBINE2D combinationPartner;
    private CardsCombinationRule currentCombination;

    private CardsBehaviour cardBehaviour;

    void Awake()
    {
        cardBehaviour = GetComponent<CardsBehaviour>();
    }

    // (TryToCombineWithNearbyCards, AttemptToStartCombination, StartCombination, CombinationTimer ��ʽ���ֲ�׃)
    // ...

    private void ResolveCombination()
    {
        if (combinationPartner == null || gameObject.GetInstanceID() > combinationPartner.gameObject.GetInstanceID())
        {
            ResetCombinationState();
            return;
        }

        if (currentCombination == null) return;

        // --- �����޸��c �_ʼ ---
        // ���N��ԭ�Ͽ���ǰ����ӛ���������λ�����Ӌ�������c
        Vector3 selfPosition = transform.position;
        // --- �����޸��c �Y�� ---


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

        // ���ɽY������
        foreach (var result in currentCombination.results)
        {
            if (Random.value <= result.probability)
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    // --- �����޸��c �_ʼ ---
                    // 1. Ӌ������λ�ã��ڮ�ǰ����λ�õĻ��A�ϣ�����ƫ��
                    Vector3 spawnPosition = selfPosition + new Vector3(-spawnOffset, 0, 0);

                    // 2. ֪ͨ߉݋��
                    CardsManager.Instance.AddCardToLogic(result.resultCard);

                    // 3. ֪ͨUI�ӣ��K���뿨�Ɣ����;��_������λ��
                    HandUI.Instance.AddCardToView(result.resultCard, spawnPosition);
                    // --- �����޸��c �Y�� ---
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

    // (������ʽ���ֲ�׃)
    // ...

    // --- ʡ��������׃�ĺ�ʽ�Թ�ʡƪ�� ---
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
        Debug.Log($"�ϳ��_ʼ: {currentCombination.combinationName}, ��Ҫ {remainingTime} ��.");
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("�ϳ����!");
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