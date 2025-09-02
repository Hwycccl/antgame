// HandUI.cs (最終簡化版 - 移除 CardContainer)
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public static HandUI Instance { get; private set; }

    // 我們不再需要 cardContainer 了
    // [SerializeField] private Transform cardContainer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 在遊戲世界中創建卡牌的視覺物件
    /// </summary>
    /// <param name="cardData">要創建的卡牌數據</param>
    /// <param name="position">創建的位置</param>
    public void AddCardToView(CardsBasicData cardData, Vector3 position)
    {
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogError("要創建的卡牌數據或其預製件為空！");
            return;
        }

        // --- 核心修改點 ---
        // 1. 直接實例化卡牌預製件，不設定父物件，這樣它就會出現在場景的最外層
        GameObject cardObject = Instantiate(cardData.cardPrefab, position, Quaternion.identity);

        // 2. （可選）給新卡牌一個有意義的名字，方便在 Hierarchy 中查看
        cardObject.name = cardData.cardName;

        // 3. 初始化卡牌上的腳本
        CardsBehaviour behaviour = cardObject.GetComponent<CardsBehaviour>();
        if (behaviour != null)
        {
            behaviour.Initialize(cardData);
        }
        else
        {
            Debug.LogError($"卡牌預製件 '{cardData.cardName}' 上沒有找到 CardsBehaviour 腳本！");
        }
    }
}