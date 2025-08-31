//
// GameManager.cs (修改後)
//
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("起始卡牌")]
    [Tooltip("遊戲開始時，直接在場上生成的卡牌")]
    [SerializeField] private List<CardsBasicData> startingCards = new List<CardsBasicData>();

    [Header("初始卡牌生成設置")]
    [SerializeField] private Vector3 startPosition = new Vector3(0, 0, 0);
    [SerializeField] private float initialSpawnSpacing = 2.0f; // 每張初始卡牌之間的間距

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
        // 遊戲開始時，生成所有起始卡牌
        for (int i = 0; i < startingCards.Count; i++)
        {
            var cardData = startingCards[i];
            if (cardData != null)
            {
                // 計算每張初始卡牌的位置
                Vector3 spawnPos = startPosition + new Vector3(i * initialSpawnSpacing, 0, 0);

                // 同時通知邏輯和UI
                CardsManager.Instance.AddCardToLogic(cardData);
                HandUI.Instance.AddCardToView(cardData, spawnPos);
            }
        }

        Debug.Log("遊戲開始！生成了 " + startingCards.Count + " 張起始卡牌");
    }
}