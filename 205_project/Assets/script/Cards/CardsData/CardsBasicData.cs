// CardsBasicData.cs 
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/RealData/basicCard")]
public class CardsBasicData : ScriptableObject
{
    [Header("基礎屬性")]
    public string cardName = "新卡牌";
    public Sprite cardImage;
    [TextArea(3, 5)] public string description;

    // --- 修改點 開始 ---
    [Header("卡牌預制體")]
    [Tooltip("對應這張卡牌數據的遊戲物件預制體")]
    public GameObject cardPrefab; // 每個卡牌數據現在直接引用它自己的預制件
    // --- 修改點 結束 ---

    [Header("卡牌類型")]
    public CardType cardType;
    public enum CardType { Ant, Resource, Building, Effect ,Enemy}

    [Header("堆疊/消耗")]
    public bool isConsumable = false;
    public bool isStackable = true;
    public int stackLimit = 10;

    [Header("資源/數值")]
    public int leafCost = 0;
    public int fungusCost = 0;
    public int fertilizer = 0;
    public int health = 0;
    public int attack = 0;
}