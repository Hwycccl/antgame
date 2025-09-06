// 放置於: CardSpawner.cs
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public static CardSpawner Instance { get; private set; }

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

    /// <summary>
    /// 根據卡牌數據在指定位置生成一張新卡牌
    /// </summary>
    /// <param name="cardData">要生成的卡牌 ScriptableObject</param>
    /// <param name="position">生成位置</param>
    /// <returns>返回生成卡牌的 Card 組件</returns>
    public Card SpawnCard(CardsBasicData cardData, Vector3 position)
    {
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogError($"無法生成卡牌：{cardData?.name} 的數據或預製件為空！");
            return null;
        }

        // 實例化預製件
        GameObject cardObject = Instantiate(cardData.cardPrefab, position, Quaternion.identity);
        cardObject.name = cardData.cardName; // 方便在場景中識別

        // 獲取 Card 組件並初始化
        Card cardController = cardObject.GetComponent<Card>();
        if (cardController != null)
        {
            cardController.Initialize(cardData);
            UnlockedCardsManager.UnlockCard(cardData.cardName);
            return cardController;
        }
        else
        {
            Debug.LogError($"卡牌預製件 {cardData.cardPrefab.name} 上沒有掛載 Card 腳本！");
            Destroy(cardObject);
            return null;
        }
    }
}
