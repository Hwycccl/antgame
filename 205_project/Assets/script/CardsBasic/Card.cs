// 放置於: Card.cs
using UnityEngine;

[RequireComponent(typeof(CardDragger), typeof(CardStacker), typeof(CardCombiner))]
public class Card : MonoBehaviour
{
    [Header("數據與顯示")]
    [SerializeField]
    private CardsBasicData _cardData;
    public CardsBasicData CardData => _cardData; // 公開的唯讀屬性

    [Tooltip("卡牌的圖片渲染器")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    // 引用其他功能組件
    public CardDragger Dragger { get; private set; }
    public CardStacker Stacker { get; private set; }
    public CardCombiner Combiner { get; private set; }

    private void Awake()
    {
        // 自動獲取同物件上的其他核心腳本
        Dragger = GetComponent<CardDragger>();
        Stacker = GetComponent<CardStacker>();
        Combiner = GetComponent<CardCombiner>();
    }

    /// <summary>
    /// 初始化卡牌，由 CardSpawner 呼叫
    /// </summary>
    public void Initialize(CardsBasicData data)
    {
        _cardData = data;
        if (artworkRenderer != null && _cardData.cardImage != null)
        {
            artworkRenderer.sprite = _cardData.cardImage;
        }
    }

    public SpriteRenderer GetArtworkRenderer()
    {
        return artworkRenderer;
    }
}
