using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������ѽ����Ŀ��ơ�
/// ����һ����̬�࣬��������Ϸ�е��κεط����ʡ�
/// </summary>
public static class UnlockedCardsManager
{
    // ʹ��һ��HashSet���洢�ѽ������Ƶ�ID�������ƣ�����ѯЧ�ʸߡ�
    private static HashSet<string> unlockedCardIDs = new HashSet<string>();

    // Ϊ�˷�����ԣ����ǿ�������Ϸ��ʼʱԤ�Ƚ���һЩ���ơ�
    // ʵ����Ϸ�У���Ӧ���ڻ���¿��Ƶ��߼��е���UnlockCard������
    static UnlockedCardsManager()
    {
        // --- ʾ����Ԥ�������ſ��� ---
        UnlockCard("QueenAnt");
        UnlockCard("WorkerMini");
        UnlockCard("WorkerMedium");
        UnlockCard("Leaf");
        UnlockCard("Scout");
        // -------------------------
    }

    /// <summary>
    /// ����һ���¿��ơ�
    /// </summary>
    /// <param name="cardID">���Ƶ�Ψһ��ʶ������������ֱ����cardName��</param>
    public static void UnlockCard(string cardID)
    {
        if (!string.IsNullOrEmpty(cardID) && !unlockedCardIDs.Contains(cardID))
        {
            unlockedCardIDs.Add(cardID);
            Debug.Log($"�����ѽ���: {cardID}");
            // ע�⣺���������ӱ����߼�������ʹ��PlayerPrefs��unlockedCardIDs�б��浽����
        }
    }

    /// <summary>
    /// ���ĳ�ſ����Ƿ��Ѿ�������
    /// </summary>
    /// <param name="cardID">Ҫ���Ŀ���ID��</param>
    /// <returns>����ѽ���������true�����򷵻�false��</returns>
    public static bool IsCardUnlocked(string cardID)
    {
        return !string.IsNullOrEmpty(cardID) && unlockedCardIDs.Contains(cardID);
    }

    /// <summary>
    /// ��ȡ�����ѽ������Ƶ�ID�б�
    /// </summary>
    /// <returns>һ�����������ѽ�������ID���б�</returns>
    public static List<string> GetUnlockedCardIDs()
    {
        return new List<string>(unlockedCardIDs);
    }
}
