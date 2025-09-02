// 放置於: GameManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("初始卡牌設置")]
    [Tooltip("遊戲開始時，在場上生成的卡牌數據列表")]
    [SerializeField] private List<CardsBasicData> startingCards;

    [Header("生成位置")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(-5f, 0, 0);
    [SerializeField] private float spawnSpacing = 2.0f; // 每張卡牌之間的間距

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SpawnInitialCards();
    }

    private void SpawnInitialCards()
    {
        if (startingCards == null || startingCards.Count == 0)
        {
            Debug.LogWarning("沒有設置任何初始卡牌！");
            return;
        }

        for (int i = 0; i < startingCards.Count; i++)
        {
            Vector3 position = spawnPosition + new Vector3(i * spawnSpacing, 0, 0);
            CardSpawner.Instance.SpawnCard(startingCards[i], position);
        }

        Debug.Log($"成功生成 {startingCards.Count} 張初始卡牌。");
    }
}