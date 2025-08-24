//CardsBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/RealData/basicCard")]
public class CardsBasicData : ScriptableObject
{
    [Header("基础属性")]
    public string cardName = "新卡牌";
    public Sprite cardImage;
    [TextArea(3, 5)] public string description;

    [Header("卡牌类型")]
    public CardType cardType;
    public enum CardType { Ant, Resource, Building, Effect }

    [Header("堆叠/消耗")]
    public bool isConsumable = false;   // 使用后是否移除
    public bool isStackable = true;     // 能否叠加
    public int stackLimit = 10;         // 最大堆叠数量

    [Header("资源/数值")]
    public int leafCost = 0;
    public int fungusCost = 0;
    public int fertilizer = 0;
    public int health = 0;
    public int attack = 0;
}
