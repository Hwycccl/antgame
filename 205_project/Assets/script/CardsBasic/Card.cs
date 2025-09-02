// �����: Card.cs
using UnityEngine;

[RequireComponent(typeof(CardDragger), typeof(CardStacker), typeof(CardCombiner))]
public class Card : MonoBehaviour
{
    [Header("�����c�@ʾ")]
    [SerializeField]
    private CardsBasicData _cardData;
    public CardsBasicData CardData => _cardData; // ���_��Ψ�x����

    [Tooltip("���ƵĈDƬ��Ⱦ��")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    // �����������ܽM��
    public CardDragger Dragger { get; private set; }
    public CardStacker Stacker { get; private set; }
    public CardCombiner Combiner { get; private set; }

    private void Awake()
    {
        // �Ԅӫ@ȡͬ����ϵ����������_��
        Dragger = GetComponent<CardDragger>();
        Stacker = GetComponent<CardStacker>();
        Combiner = GetComponent<CardCombiner>();
    }

    /// <summary>
    /// ��ʼ�����ƣ��� CardSpawner ����
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
