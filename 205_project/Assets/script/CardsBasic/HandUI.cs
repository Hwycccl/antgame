// HandUI.cs 
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance;
    [SerializeField] private Transform handArea; // 用來擺放初始卡牌的父物件

    // 我們不再需要手動映射預制件，所以移除了舊的字典和Prefab欄位
    private List<GameObject> cardsOnField = new List<GameObject>(); // 將 handCards 更名為 cardsOnField

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // --- 核心修改點 開始 ---
    /// <summary>
    /// 將單張卡牌的視覺物件顯示到指定的場上位置
    /// </summary>
    /// <param name="cardData">要生成的卡牌數據</param>
    /// <param name="spawnPosition">生成位置的世界座標</param>
    public void AddCardToView(CardsBasicData cardData, Vector3 spawnPosition)
    {
        // 1. 檢查卡牌數據中的預制件是否已設定
        if (cardData.cardPrefab == null)
        {
            Debug.LogError($"卡牌 {cardData.cardName} 的數據中沒有指定 Card Prefab！");
            return;
        }

        // 2. 直接使用卡牌數據中指定的預制件，並在指定位置生成
        GameObject cardObj = Instantiate(cardData.cardPrefab, spawnPosition, Quaternion.identity);

        // 可選：如果你希望所有卡牌都在一個統一的父物件下管理，可以取消下面這行的註解
        // cardObj.transform.SetParent(handArea);

        var cardBehaviour = cardObj.GetComponent<CardsBehaviour>();
        if (cardBehaviour != null)
        {
            cardBehaviour.Initialize(cardData);
        }
        else
        {
            Debug.LogWarning($"預制體 {cardData.cardPrefab.name} 上沒有 CardsBehaviour 組件");
        }

        cardsOnField.Add(cardObj);
    }
    // --- 核心修改點 結束 ---


    /// <summary>
    /// 清空場上所有卡牌的顯示
    /// </summary>
    public void ClearHand()
    {
        foreach (GameObject card in cardsOnField)
        {
            Destroy(card);
        }
        cardsOnField.Clear();
    }
}