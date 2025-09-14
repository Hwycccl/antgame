// 文件名: CardSpawner.cs (修改后)
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public static CardSpawner Instance { get; private set; }

    private void Awake()
    {
        // ... (单例模式代码不变) ...
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // --- 核心修改 ---
    public Card SpawnCard(CardsBasicData cardData, Vector3 position)
    {
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogError($"无法生成卡牌：{cardData?.name} 的数据或预制件为空！");
            return null;
        }

        // 从对象池获取卡牌，而不是Instantiate
        Card cardController = CardPool.Instance.Get(cardData.cardPrefab, position, Quaternion.identity);

        if (cardController != null)
        {
            cardController.Initialize(cardData);
            // --- 【在这里添加修复代码】 ---
            // 确保无论何时生成卡牌，其拖拽功能都是开启的
            if (cardController.Dragger != null)
            {
                cardController.Dragger.enabled = true;
            }
            // --- 【修复代码结束】 ---
            UnlockedCardsManager.UnlockCard(cardData.cardName);
            return cardController;
        }
        else
        {
            // 理论上，如果对象池逻辑正确，这里不会执行
            Debug.LogError($"卡牌预制件 {cardData.cardPrefab.name} 上没有挂载 Card 脚本！");
            return null;
        }
    }
}