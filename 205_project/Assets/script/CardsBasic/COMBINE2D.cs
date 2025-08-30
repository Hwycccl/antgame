//COMBINE2D.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class COMBINE2D : MonoBehaviour
{
    [Header("����䷽���ݿ�")]
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
    /// �����븽���Ŀ��ƽ��кϳɣ����Ǻϳɵķ����
    /// </summary>
    /// <returns>����ɹ��ҵ��ϳɶ��󲢿�ʼ�ϳɣ��򷵻�true</returns>
    public bool TryToCombineWithNearbyCards()
    {
        // ����Ѿ��ںϳ��У���ֱ�ӷ���
        if (IsInCombination()) return false;

        // ����һ�����뾶��Ѱ�Ҹ����Ŀ���
        float detectionRadius = 1.5f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // �ų��Լ�
            if (hitCollider.gameObject == gameObject) continue;

            var otherCombineScript = hitCollider.GetComponent<COMBINE2D>();
            // ���Է��Ƿ������û���ںϳ���
            if (otherCombineScript != null && !otherCombineScript.IsInCombination())
            {
                // ���������ſ��ƿ�ʼ�ϳ�
                if (AttemptToStartCombination(otherCombineScript))
                {
                    return true; // �ɹ��ҵ�����ʼ�ϳ�
                }
            }
        }

        return false; // û���ҵ����ԺϳɵĶ���
    }

    /// <summary>
    /// ������һ�������Ŀ�꿨�ƿ�ʼ�ϳ�����
    /// </summary>
    private bool AttemptToStartCombination(COMBINE2D targetCombine)
    {
        if (combinationData == null) return false;

        var card1Data = cardBehaviour.GetCardData();
        var card2Data = targetCombine.cardBehaviour.GetCardData();

        if (card1Data == null || card2Data == null) return false;

        var inputCards = new List<CardsBasicData> { card1Data, card2Data };
        var combination = combinationData.GetCombination(inputCards);

        // ����ҵ���ƥ��ĺϳɹ���
        if (combination != null)
        {
            // ���Լ��ͶԷ����϶������ϳ�����
            StartCombination(combination, targetCombine);
            targetCombine.StartCombination(combination, this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// ���úϳ�״̬��������ʱ��
    /// </summary>
    private void StartCombination(CardsCombinationRule combination, COMBINE2D partner)
    {
        currentCombination = combination;
        combinationPartner = partner;
        remainingTime = currentCombination.time;

        // ֹͣ���ܴ��ڵľɼ�ʱ��
        if (combinationCoroutine != null)
        {
            StopCoroutine(combinationCoroutine);
        }
        combinationCoroutine = StartCoroutine(CombinationTimer());
    }

    /// <summary>
    /// �ϳɵ���ʱЭ��
    /// </summary>
    private IEnumerator CombinationTimer()
    {
        Debug.Log($"�ϳɿ�ʼ: {currentCombination.combinationName}, ��Ҫ {remainingTime} ��.");
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            // �������������UI����ʾʣ��ʱ��
            yield return null;
        }

        Debug.Log("�ϳ����!");
        ResolveCombination();
    }

    /// <summary>
    /// ����ϳɽ��������ԭ�ϲ������¿���
    /// </summary>
    private void ResolveCombination()
    {
        if (currentCombination == null || combinationPartner == null) return;

        // �����Ҫ���ٵĿ���
        bool destroySelf = false;
        bool destroyPartner = false;

        var selfData = cardBehaviour.GetCardData();
        var partnerData = combinationPartner.cardBehaviour.GetCardData();

        foreach (var req in currentCombination.requiredCards)
        {
            if (req.destroyOnCombine)
            {
                if (req.specificCard != null) // ���ض�����ƥ��
                {
                    if (selfData == req.specificCard) destroySelf = true;
                    if (partnerData == req.specificCard) destroyPartner = true;
                }
                else // ����������ƥ��
                {
                    if (selfData.cardType == req.cardType) destroySelf = true;
                    if (partnerData.cardType == req.cardType) destroyPartner = true;
                }
            }
        }

        // ���ɽ������
        foreach (var result in currentCombination.results)
        {
            if (Random.value <= result.probability) // ����������
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    HandUI.Instance.AddCardToHand(result.resultCard);
                }
            }
        }

        // ��ȡ������Ϸ�����Է����ȱ�����
        GameObject partnerObject = combinationPartner.gameObject;

        // ���ٿ���
        if (destroySelf)
        {
            Destroy(gameObject);
        }
        if (destroyPartner)
        {
            Destroy(partnerObject);
        }

        // ����״̬
        if (!destroySelf) ResetCombinationState();
        if (!destroyPartner && partnerObject != null)
            partnerObject.GetComponent<COMBINE2D>().ResetCombinationState();

    }

    /// <summary>
    /// ���úϳ�״̬
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
    /// ��鵱ǰ�����Ƿ����ںϳ���
    /// </summary>
    public bool IsInCombination() => currentCombination != null;
}
