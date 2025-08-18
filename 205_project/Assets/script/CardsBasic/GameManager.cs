using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("起始卡组")]
    [SerializeField] private List<CardsBasicData> startingDeck = new List<CardsBasicData>();

    [Header("起始手牌数量")]
    [SerializeField] private int startingHandSize = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 1. 初始化卡组
        CardsManager.Instance.InitializePlayerDeck(startingDeck);

        // 2. 抽起始手牌
        for (int i = 0; i < startingHandSize; i++)
        {
            CardsBasicData card = CardsManager.Instance.DrawCard();
            if (card != null)
            {
                HandUI.Instance.AddCardToHand(card);  // ? UI 层负责显示
            }
        }

        Debug.Log("游戏开始！发了 " + startingHandSize + " 张手牌");
    }

    /// <summary>
    /// 结束回合（你可以在这里清空手牌、弃掉不用的牌等）
    /// </summary>
    public void EndTurn()
    {
        Debug.Log("回合结束，弃掉所有手牌");

        List<CardsBasicData> hand = CardsManager.Instance.GetPlayerHand();

        foreach (CardsBasicData card in hand)
        {
            CardsManager.Instance.UseCard(card); // 或者写个 DiscardCard 方法
        }

        HandUI.Instance.ClearHand();
    }

    /// <summary>
    /// 开始新回合（重新抽牌）
    /// </summary>
    public void StartTurn()
    {
        Debug.Log("新回合开始，抽 " + startingHandSize + " 张牌");

        for (int i = 0; i < startingHandSize; i++)
        {
            CardsBasicData card = CardsManager.Instance.DrawCard();
            if (card != null)
            {
                HandUI.Instance.AddCardToHand(card);
            }
        }
    }
}
