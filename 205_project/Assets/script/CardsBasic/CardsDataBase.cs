//CardsDataBase
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌数据库，用于存储和管理所有卡牌数据资源
/// </summary>
public class CardsDataBase : MonoBehaviour
{
    public static CardsDataBase Instance { get; private set; }

    [Header("卡牌资源列表")]
    [SerializeField] public List<CardsBasicData> allCards = new List<CardsBasicData>();

    private Dictionary<string, CardsBasicData> cardDictionary = new Dictionary<string, CardsBasicData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    /// <summary>
    /// 初始化卡牌数据库
    /// </summary>
    private void InitializeDatabase()
    {
        cardDictionary.Clear();
        foreach (var card in allCards)
        {
            if (card != null && !cardDictionary.ContainsKey(card.cardName))
            {
                cardDictionary.Add(card.cardName, card);
            }
        }
    }

    /// <summary>
    /// 通过卡牌名称获取卡牌数据
    /// </summary>
    public CardsBasicData GetCardData(string cardName)
    {
        if (cardDictionary.TryGetValue(cardName, out var cardData))
        {
            return cardData;
        }
        Debug.LogWarning($"卡牌 {cardName} 不存在于数据库中");
        return null;
    }

    /// <summary>
    /// 获取所有卡牌数据
    /// </summary>
    public List<CardsBasicData> GetAllCards()
    {
        return new List<CardsBasicData>(allCards);
    }

    /// <summary>
    /// 添加新卡牌到数据库
    /// </summary>
    public void AddCard(CardsBasicData newCard)
    {
        if (newCard != null && !allCards.Contains(newCard))
        {
            allCards.Add(newCard);
            cardDictionary[newCard.cardName] = newCard;
        }
    }

    /// <summary>
    /// 根据类型筛选卡牌
    /// </summary>
    public List<CardsBasicData> GetCardsByType(CardsBasicData.CardType type)
    {
        return allCards.FindAll(card => card.cardType == type);
    }
}