using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("��ʼ����")]
    [SerializeField] private List<CardsBasicData> startingDeck = new List<CardsBasicData>();

    [Header("��ʼ��������")]
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
        // 1. ��ʼ������
        CardsManager.Instance.InitializePlayerDeck(startingDeck);

        // 2. ����ʼ����
        for (int i = 0; i < startingHandSize; i++)
        {
            CardsBasicData card = CardsManager.Instance.DrawCard();
            if (card != null)
            {
                HandUI.Instance.AddCardToHand(card);  // ? UI �㸺����ʾ
            }
        }

        Debug.Log("��Ϸ��ʼ������ " + startingHandSize + " ������");
    }

    /// <summary>
    /// �����غϣ������������������ơ��������õ��Ƶȣ�
    /// </summary>
    public void EndTurn()
    {
        Debug.Log("�غϽ�����������������");

        List<CardsBasicData> hand = CardsManager.Instance.GetPlayerHand();

        foreach (CardsBasicData card in hand)
        {
            CardsManager.Instance.UseCard(card); // ����д�� DiscardCard ����
        }

        HandUI.Instance.ClearHand();
    }

    /// <summary>
    /// ��ʼ�»غϣ����³��ƣ�
    /// </summary>
    public void StartTurn()
    {
        Debug.Log("�»غϿ�ʼ���� " + startingHandSize + " ����");

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
