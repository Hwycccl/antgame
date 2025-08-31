// CardsManager.cs 
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 引入Linq以便于查询

/// <summary>
/// 改造后的卡牌管理器，负责追踪场上所有卡牌并动态计算资源。
/// 游戏逻辑的核心（大脑）。
/// </summary>
public class CardsManager : MonoBehaviour
{
    public static CardsManager Instance { get; private set; }

    // 移除了牌库和弃牌堆，现在只追踪场上所有的卡牌数据
    [SerializeField] private List<CardsBasicData> cardsOnField = new List<CardsBasicData>();

    // 公开的资源量，方便UI脚本读取
    public int FungusAmount { get; private set; }
    public int LeafFragmentAmount { get; private set; }
    public int FecesAmount { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 游戏开始时计算一次初始资源
        CalculateResourceTotals();
    }

    /// <summary>
    /// 将一张卡牌的“数据”添加到场上逻辑中
    /// </summary>
    public void AddCardToLogic(CardsBasicData card)
    {
        if (card != null)
        {
            cardsOnField.Add(card);
            // 每当卡牌数量变化时，都重新计算总资源
            CalculateResourceTotals();
        }
    }

    /// <summary>
    /// 从场上逻辑中移除一张卡牌的“数据”
    /// </summary>
    public void RemoveCardFromLogic(CardsBasicData card)
    {
        if (card != null && cardsOnField.Contains(card))
        {
            cardsOnField.Remove(card);
            // 每当卡牌数量变化时，都重新计算总资源
            CalculateResourceTotals();
        }
    }

    /// <summary>
    /// 核心功能：遍历场上所有卡牌，计算每种资源的-总和
    /// </summary>
    private void CalculateResourceTotals()
    {
        // 先清零
        FungusAmount = 0;
        LeafFragmentAmount = 0;
        FecesAmount = 0;

        foreach (CardsBasicData card in cardsOnField)
        {
            // 检查这张卡是否是资源卡
            if (card is ResourceBasicData resourceCard)
            {
                // 根据资源类型累加其价值
                switch (resourceCard.resourceType)
                {
                    case ResourceBasicData.ResourceType.Fungus:
                        FungusAmount += resourceCard.resourceValue;
                        break;
                    case ResourceBasicData.ResourceType.LeafFragment:
                        LeafFragmentAmount += resourceCard.resourceValue;
                        break;
                    case ResourceBasicData.ResourceType.Feces:
                        FecesAmount += resourceCard.resourceValue;
                        break;
                }
            }
        }

        // 可以在这里触发一个事件，通知UI更新
        // 为了简单，我们让UI脚本在Update里自己来读
        Debug.Log($"资源更新: 真菌={FungusAmount}, 碎叶={LeafFragmentAmount}, 粪便={FecesAmount}");
    }
}