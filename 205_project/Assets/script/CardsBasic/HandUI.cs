// HandUI.cs 
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;
    [SerializeField] private Transform handArea; // �Á�[�ų�ʼ���Ƶĸ����

    // �҂�������Ҫ�ք�ӳ���A�Ƽ��������Ƴ����f���ֵ��Prefab��λ
    private List<GameObject> cardsOnField = new List<GameObject>(); // �� handCards ������ cardsOnField

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // --- �����޸��c �_ʼ ---
    /// <summary>
    /// ���Ώ����Ƶ�ҕ�X����@ʾ��ָ���Ĉ���λ��
    /// </summary>
    /// <param name="cardData">Ҫ���ɵĿ��Ɣ���</param>
    /// <param name="spawnPosition">����λ�õ���������</param>
    public void AddCardToView(CardsBasicData cardData, Vector3 spawnPosition)
    {
        // 1. �z�鿨�Ɣ����е��A�Ƽ��Ƿ����O��
        if (cardData.cardPrefab == null)
        {
            Debug.LogError($"���� {cardData.cardName} �Ĕ����Л]��ָ�� Card Prefab��");
            return;
        }

        // 2. ֱ��ʹ�ÿ��Ɣ�����ָ�����A�Ƽ����K��ָ��λ������
        GameObject cardObj = Instantiate(cardData.cardPrefab, spawnPosition, Quaternion.identity);

        // ���x�������ϣ�����п��ƶ���һ���yһ�ĸ�����¹�������ȡ�������@�е��]��
        // cardObj.transform.SetParent(handArea);

        var cardBehaviour = cardObj.GetComponent<CardsBehaviour>();
        if (cardBehaviour != null)
        {
            cardBehaviour.Initialize(cardData);
        }
        else
        {
            Debug.LogWarning($"�A���w {cardData.cardPrefab.name} �ϛ]�� CardsBehaviour �M��");
        }

        cardsOnField.Add(cardObj);
    }
    // --- �����޸��c �Y�� ---


    /// <summary>
    /// ��Ո������п��Ƶ��@ʾ
    /// </summary>
    public void ClearHand()
    {
        foreach (GameObject card in cardsOnField)
        {
            Destroy(card);
        }
        cardsOnField.Clear();
    }
}