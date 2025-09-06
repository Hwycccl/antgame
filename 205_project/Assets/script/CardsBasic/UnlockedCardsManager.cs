using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理玩家已解锁的卡牌。
/// 这是一个静态类，可以在游戏中的任何地方访问。
/// </summary>
public static class UnlockedCardsManager
{
    // 使用一个HashSet来存储已解锁卡牌的ID（或名称），查询效率高。
    private static HashSet<string> unlockedCardIDs = new HashSet<string>();

    // 为了方便测试，我们可以在游戏开始时预先解锁一些卡牌。
    // 实际游戏中，你应该在获得新卡牌的逻辑中调用UnlockCard方法。
    static UnlockedCardsManager()
    {
        // --- 示例：预解锁几张卡牌 ---
        UnlockCard("QueenAnt");
        UnlockCard("WorkerMini");
        UnlockCard("WorkerMedium");
        UnlockCard("Leaf");
        UnlockCard("Scout");
        // -------------------------
    }

    /// <summary>
    /// 解锁一张新卡牌。
    /// </summary>
    /// <param name="cardID">卡牌的唯一标识符，这里我们直接用cardName。</param>
    public static void UnlockCard(string cardID)
    {
        if (!string.IsNullOrEmpty(cardID) && !unlockedCardIDs.Contains(cardID))
        {
            unlockedCardIDs.Add(cardID);
            Debug.Log($"卡牌已解锁: {cardID}");
            // 注意：这里可以添加保存逻辑，例如使用PlayerPrefs将unlockedCardIDs列表保存到本地
        }
    }

    /// <summary>
    /// 检查某张卡牌是否已经解锁。
    /// </summary>
    /// <param name="cardID">要检查的卡牌ID。</param>
    /// <returns>如果已解锁，返回true；否则返回false。</returns>
    public static bool IsCardUnlocked(string cardID)
    {
        return !string.IsNullOrEmpty(cardID) && unlockedCardIDs.Contains(cardID);
    }

    /// <summary>
    /// 获取所有已解锁卡牌的ID列表。
    /// </summary>
    /// <returns>一个包含所有已解锁卡牌ID的列表。</returns>
    public static List<string> GetUnlockedCardIDs()
    {
        return new List<string>(unlockedCardIDs);
    }
}
