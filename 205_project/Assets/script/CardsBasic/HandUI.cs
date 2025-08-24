using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;   // 单例
    [SerializeField] private Transform handArea;   // 用来摆放卡牌的父物体

    [Header("卡牌预制体映射")]
    [SerializeField] private GameObject antCardPrefab;
    [SerializeField] private GameObject resourceCardPrefab;
    [SerializeField] private GameObject buildingCardPrefab;
    [SerializeField] private GameObject effectCardPrefab;

    private Dictionary<CardsBasicData.CardType, GameObject> prefabMap;

    private List<GameObject> handCards = new List<GameObject>(); // 保存当前手牌实例

    private void Awake()
    {
        // 单例
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // 建立类型和预制体的映射
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
        DrawInitialHand(1); // 初始发 1张牌
    }

    /// <summary>
    /// 抽多张牌
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
    /// 将单张卡牌显示到手牌区
    /// </summary>
    public void AddCardToHand(CardsBasicData cardData)
    {
        if (!prefabMap.ContainsKey(cardData.cardType))
        {
            Debug.LogWarning($"未找到 {cardData.cardType} 类型的预制体映射");
            return;
        }

        GameObject prefabToUse = prefabMap[cardData.cardType];
        GameObject cardObj = Instantiate(prefabToUse, handArea);
        // 确保位置和缩放正确
        cardObj.transform.localPosition = Vector3.zero;  // 相对于 handArea 的本地位置
        cardObj.transform.localScale = Vector3.one;      // 原始大小，不会被缩放到 0

        var cardBehaviour = cardObj.GetComponent<CardsBehaviour>();
        if (cardBehaviour != null)
        {
            cardBehaviour.Initialize(cardData);
        }
        else
        {
            Debug.LogWarning($"预制体 {prefabToUse.name} 上没有 CardsBehaviour 组件");
        }

        handCards.Add(cardObj); // 保存实例
    }

    /// <summary>
    /// 清空手牌显示（通常在回合结束）
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
