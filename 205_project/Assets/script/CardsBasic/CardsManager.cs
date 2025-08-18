//CardsManager
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌管理器，处理卡牌的获取、使用和游戏逻辑
/// </summary>
public class CardsManager : MonoBehaviour
{
    public static CardsManager Instance { get; private set; }

    [Header("玩家卡牌")]
    [SerializeField] private List<CardsBasicData> playerDeck = new List<CardsBasicData>();
    [SerializeField] private List<CardsBasicData> playerHand = new List<CardsBasicData>();
    [SerializeField] private List<CardsBasicData> playerDiscardPile = new List<CardsBasicData>();

    [Header("资源")]
    public int leafResources = 10;
    public int fungusResources = 5;
    public int fertilizerResources = 2;

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

    /// <summary>
    /// 初始化玩家卡组
    /// </summary>
    public void InitializePlayerDeck(List<CardsBasicData> startingDeck)
    {
        playerDeck = new List<CardsBasicData>(startingDeck);
        ShuffleDeck();
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    public void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            CardsBasicData temp = playerDeck[i];
            int randomIndex = UnityEngine.Random.Range(i, playerDeck.Count); // 明确使用UnityEngine.Random
            playerDeck[i] = playerDeck[randomIndex];
            playerDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 抽牌
    /// </summary>
    public CardsBasicData DrawCard()
    {
        if (playerDeck.Count == 0)
        {
            if (playerDiscardPile.Count == 0)
            {
                Debug.LogWarning("牌库和弃牌堆都为空，无法抽牌");
                return null;
            }
            // 重新洗入弃牌堆
            playerDeck.AddRange(playerDiscardPile);
            playerDiscardPile.Clear();
            ShuffleDeck();
        }

        CardsBasicData drawnCard = playerDeck[0];
        playerDeck.RemoveAt(0);
        playerHand.Add(drawnCard);
        return drawnCard;
    }

    /// <summary>
    /// 使用卡牌
    /// </summary>
    public bool UseCard(CardsBasicData card)
    {
        if (!playerHand.Contains(card))
        {
            Debug.LogWarning("尝试使用不在手牌中的卡牌");
            return false;
        }

        // 检查资源是否足够
        if (leafResources < card.leafCost ||
            fungusResources < card.fungusCost ||
            fertilizerResources < card.fertilizer)
        {
            Debug.Log("资源不足，无法使用此卡牌");
            return false;
        }

        // 扣除资源
        leafResources -= card.leafCost;
        fungusResources -= card.fungusCost;
        fertilizerResources -= card.fertilizer;

        // 处理卡牌效果
        // 这里可以添加具体的效果逻辑

        // 从手牌移除
        playerHand.Remove(card);

        // 如果不是消耗品，放入弃牌堆
        if (!card.isConsumable)
        {
            playerDiscardPile.Add(card);
        }

        return true;
    }

    /// <summary>
    /// 获取当前手牌
    /// </summary>
    public List<CardsBasicData> GetPlayerHand()
    {
        return new List<CardsBasicData>(playerHand);
    }

    /// <summary>
    /// 获取当前牌库数量
    /// </summary>
    public int GetDeckCount()
    {
        return playerDeck.Count;
    }

    /// <summary>
    /// 获取当前弃牌堆数量
    /// </summary>
    public int GetDiscardPileCount()
    {
        return playerDiscardPile.Count;
    }
}