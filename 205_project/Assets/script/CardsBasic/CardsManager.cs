//CardsManager
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ƹ������������ƵĻ�ȡ��ʹ�ú���Ϸ�߼�
/// </summary>
public class CardsManager : MonoBehaviour
{
    public static CardsManager Instance { get; private set; }

    [Header("��ҿ���")]
    [SerializeField] private List<CardsBasicData> playerDeck = new List<CardsBasicData>();
    [SerializeField] private List<CardsBasicData> playerHand = new List<CardsBasicData>();
    [SerializeField] private List<CardsBasicData> playerDiscardPile = new List<CardsBasicData>();

    [Header("��Դ")]
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
    /// ��ʼ����ҿ���
    /// </summary>
    public void InitializePlayerDeck(List<CardsBasicData> startingDeck)
    {
        playerDeck = new List<CardsBasicData>(startingDeck);
        ShuffleDeck();
    }

    /// <summary>
    /// ϴ��
    /// </summary>
    public void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            CardsBasicData temp = playerDeck[i];
            int randomIndex = UnityEngine.Random.Range(i, playerDeck.Count); // ��ȷʹ��UnityEngine.Random
            playerDeck[i] = playerDeck[randomIndex];
            playerDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public CardsBasicData DrawCard()
    {
        if (playerDeck.Count == 0)
        {
            if (playerDiscardPile.Count == 0)
            {
                Debug.LogWarning("�ƿ�����ƶѶ�Ϊ�գ��޷�����");
                return null;
            }
            // ����ϴ�����ƶ�
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
    /// ʹ�ÿ���
    /// </summary>
    public bool UseCard(CardsBasicData card)
    {
        if (!playerHand.Contains(card))
        {
            Debug.LogWarning("����ʹ�ò��������еĿ���");
            return false;
        }

        // �����Դ�Ƿ��㹻
        if (leafResources < card.leafCost ||
            fungusResources < card.fungusCost ||
            fertilizerResources < card.fertilizer)
        {
            Debug.Log("��Դ���㣬�޷�ʹ�ô˿���");
            return false;
        }

        // �۳���Դ
        leafResources -= card.leafCost;
        fungusResources -= card.fungusCost;
        fertilizerResources -= card.fertilizer;

        // ������Ч��
        // ���������Ӿ����Ч���߼�

        // �������Ƴ�
        playerHand.Remove(card);

        // �����������Ʒ���������ƶ�
        if (!card.isConsumable)
        {
            playerDiscardPile.Add(card);
        }

        return true;
    }

    /// <summary>
    /// ��ȡ��ǰ����
    /// </summary>
    public List<CardsBasicData> GetPlayerHand()
    {
        return new List<CardsBasicData>(playerHand);
    }

    /// <summary>
    /// ��ȡ��ǰ�ƿ�����
    /// </summary>
    public int GetDeckCount()
    {
        return playerDeck.Count;
    }

    /// <summary>
    /// ��ȡ��ǰ���ƶ�����
    /// </summary>
    public int GetDiscardPileCount()
    {
        return playerDiscardPile.Count;
    }
}