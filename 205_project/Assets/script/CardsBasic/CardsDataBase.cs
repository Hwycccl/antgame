//CardsDataBase
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ݿ⣬���ڴ洢�͹������п���������Դ
/// </summary>
public class CardsDataBase : MonoBehaviour
{
    public static CardsDataBase Instance { get; private set; }

    [Header("������Դ�б�")]
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
    /// ��ʼ���������ݿ�
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
    /// ͨ���������ƻ�ȡ��������
    /// </summary>
    public CardsBasicData GetCardData(string cardName)
    {
        if (cardDictionary.TryGetValue(cardName, out var cardData))
        {
            return cardData;
        }
        Debug.LogWarning($"���� {cardName} �����������ݿ���");
        return null;
    }

    /// <summary>
    /// ��ȡ���п�������
    /// </summary>
    public List<CardsBasicData> GetAllCards()
    {
        return new List<CardsBasicData>(allCards);
    }

    /// <summary>
    /// ����¿��Ƶ����ݿ�
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
    /// ��������ɸѡ����
    /// </summary>
    public List<CardsBasicData> GetCardsByType(CardsBasicData.CardType type)
    {
        return allCards.FindAll(card => card.cardType == type);
    }
}