using System.Collections;
using System.Collections.Generic;
// CardData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/RealData/basicCard")]//新建一个数据文件
public class CardsBasicData : ScriptableObject//可以被多个地方引用，内存里只有一份，省资源
{
    [Header("基础属性")]
    public string cardName = "新卡牌";
    public Sprite cardImage;
    [TextArea(3, 5)] public string description;

    [Header("卡牌类型")]
    public CardType cardType;
    public enum CardType { Ant, Resource, Building, Effect }

    [Header("资源消耗")]
    public int leafCost = 0;
    public int fungusCost = 0;
    public int fertilizer = 0;

    [Header("特殊属性")]
    public int health;
    public int attack;

    [Header("行为参数")]
    public bool isConsumable = false;//使用后是否移除
    public bool isStackable = true;//能否重叠
}