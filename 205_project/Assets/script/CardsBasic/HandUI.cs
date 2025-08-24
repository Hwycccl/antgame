using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;   // ����
    [SerializeField] private Transform handArea;   // �����ڷſ��Ƶĸ�����

    [Header("����Ԥ����ӳ��")]
    [SerializeField] private GameObject antCardPrefab;
    [SerializeField] private GameObject resourceCardPrefab;
    [SerializeField] private GameObject buildingCardPrefab;
    [SerializeField] private GameObject effectCardPrefab;

    private Dictionary<CardsBasicData.CardType, GameObject> prefabMap;

    private List<GameObject> handCards = new List<GameObject>(); // ���浱ǰ����ʵ��

    private void Awake()
    {
        // ����
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // �������ͺ�Ԥ�����ӳ��
        prefabMap = new Dictionary<CardsBasicData.CardType, GameObject>
        {
            { CardsBasicData.CardType.Ant, antCardPrefab },
            { CardsBasicData.CardType.Resource, resourceCardPrefab },
            { CardsBasicData.CardType.Building, buildingCardPrefab },
            { CardsBasicData.CardType.Effect, effectCardPrefab }
        };
    }

    private void Start()
    {
        DrawInitialHand(1); // ��ʼ�� 1����
    }

    /// <summary>
    /// �������
    /// </summary>
    public void DrawInitialHand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var cardData = CardsManager.Instance.DrawCard();
            if (cardData != null)
                AddCardToHand(cardData);
        }
    }

    /// <summary>
    /// �����ſ�����ʾ��������
    /// </summary>
    public void AddCardToHand(CardsBasicData cardData)
    {
        if (!prefabMap.ContainsKey(cardData.cardType))
        {
            Debug.LogWarning($"δ�ҵ� {cardData.cardType} ���͵�Ԥ����ӳ��");
            return;
        }

        GameObject prefabToUse = prefabMap[cardData.cardType];
        GameObject cardObj = Instantiate(prefabToUse, handArea);
        // ȷ��λ�ú�������ȷ
        cardObj.transform.localPosition = Vector3.zero;  // ����� handArea �ı���λ��
        cardObj.transform.localScale = Vector3.one;      // ԭʼ��С�����ᱻ���ŵ� 0

        var cardBehaviour = cardObj.GetComponent<CardsBehaviour>();
        if (cardBehaviour != null)
        {
            cardBehaviour.Initialize(cardData);
        }
        else
        {
            Debug.LogWarning($"Ԥ���� {prefabToUse.name} ��û�� CardsBehaviour ���");
        }

        handCards.Add(cardObj); // ����ʵ��
    }

    /// <summary>
    /// ���������ʾ��ͨ���ڻغϽ�����
    /// </summary>
    public void ClearHand()
    {
        foreach (GameObject card in handCards)
        {
            Destroy(card);
        }
        handCards.Clear();
    }
}
