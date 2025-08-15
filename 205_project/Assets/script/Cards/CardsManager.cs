using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// CardsManager.cs

public class CardsManager : MonoBehaviour
{
    public static CardsManager Instance;

    [SerializeField]
    private List<CardsBasicData> _allCards = new List<CardsBasicData>();

    private Dictionary<string, CardsBasicData> _cardDictionary;

    void Awake()
    {
        Instance = this;
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _cardDictionary = new Dictionary<string, CardsBasicData>();
        foreach (CardsBasicData card in _allCards)
        {
            _cardDictionary.Add(card.name, card);
        }
    }

    public CardsBasicData GetCardByName(string cardName)
    {
        if (_cardDictionary.TryGetValue(cardName, out CardsBasicData card))
        {
            return card;
        }
        Debug.LogError($"¿¨ÅÆ {cardName} ²»´æÔÚ!");
        return null;
    }

    public List<CardsBasicData> GetCardsByType(CardsBasicData.CardType type)
    {
        return _allCards.FindAll(card => card.cardType == type);
    }
}