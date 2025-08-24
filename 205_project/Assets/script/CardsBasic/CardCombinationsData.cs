// CardCombinationsData.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 定义组合中对单张必需卡牌的要求
/// </summary>
[System.Serializable]
public class RequiredCard
{
    [Tooltip("必需的卡牌数据")]
    public CardsBasicData cardData;
    [Tooltip("组合成功后是否销毁这张卡牌")]
    public bool destroyOnCombine = true;
}

/// <summary>
/// 定义单个卡牌组合的规则
/// </summary>
[System.Serializable]
public class CardCombination
{
    public string combinationName;

    [Tooltip("组合所需的两张卡牌以及它们的销毁选项")]
    public List<RequiredCard> requiredCards = new List<RequiredCard>(2);

    [Tooltip("组合成功后可能生成的额外卡牌")]
    public List<CardsBasicData> resultingCards = new List<CardsBasicData>();

    [Tooltip("组合生成新卡牌所需的等待时间（秒）")]
    public float time = 0f;
}

/// <summary>
/// 存储所有卡牌组合的数据库 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "CardCombinations", menuName = "AntGame/Card Combinations Data")]
public class CardCombinationsData : ScriptableObject
{
    [Header("卡牌组合列表")]
    public List<CardCombination> combinations = new List<CardCombination>();

    /// <summary>
    /// 检查两张卡牌是否匹配某个组合，并返回完整的组合数据 (已修正逻辑)
    /// </summary>
    public CardCombination GetCombination(CardsBasicData card1, CardsBasicData card2)
    {
        foreach (var combination in combinations)
        {
            if (combination.requiredCards.Count != 2) continue;

            // 获取规则中定义的两种卡牌数据
            CardsBasicData required1 = combination.requiredCards[0].cardData;
            CardsBasicData required2 = combination.requiredCards[1].cardData;

            // 检查传入的卡牌是否与规则匹配（允许顺序颠倒）
            bool match = (card1 == required1 && card2 == required2) ||
                         (card1 == required2 && card2 == required1);

            if (match)
            {
                return combination;
            }
        }
        return null; // 没有找到匹配项
    }
}