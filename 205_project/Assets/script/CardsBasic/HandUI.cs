// HandUI.cs (��K������ - �Ƴ� CardContainer)
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance { get; private set; }

    // �҂�������Ҫ cardContainer ��
    // [SerializeField] private Transform cardContainer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// ���[�������Є������Ƶ�ҕ�X���
    /// </summary>
    /// <param name="cardData">Ҫ�����Ŀ��Ɣ���</param>
    /// <param name="position">������λ��</param>
    public void AddCardToView(CardsBasicData cardData, Vector3 position)
    {
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogError("Ҫ�����Ŀ��Ɣ��������A�u����գ�");
            return;
        }

        // --- �����޸��c ---
        // 1. ֱ�ӌ����������A�u�������O����������@�����͕����F�ڈ����������
        GameObject cardObject = Instantiate(cardData.cardPrefab, position, Quaternion.identity);

        // 2. �����x���o�¿���һ�������x�����֣������� Hierarchy �в鿴
        cardObject.name = cardData.cardName;

        // 3. ��ʼ�������ϵ��_��
        CardsBehaviour behaviour = cardObject.GetComponent<CardsBehaviour>();
        if (behaviour != null)
        {
            behaviour.Initialize(cardData);
        }
        else
        {
            Debug.LogError($"�����A�u�� '{cardData.cardName}' �ϛ]���ҵ� CardsBehaviour �_����");
        }
    }
}