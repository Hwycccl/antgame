using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 定义组合所需的一类卡牌（可指定具体卡 or 卡牌类型）
/// </summary>
[System.Serializable]
public class RequiredCardGroup
{
    [Tooltip("指定需要的具体卡牌数据，如果为空则按类型匹配")]
    public CardsBasicData specificCard;

    [Tooltip("需要的卡牌类型（如果 specificCard 不为空则忽略）")]
    public CardsBasicData.CardType cardType;

    [Tooltip("需要的卡牌数量（>=1）")]
    public int requiredCount = 1;

    [Tooltip("合成时是否销毁这些卡牌")]
    public bool destroyOnCombine = true;

    public bool Matches(List<CardsBasicData> inputs)
    {
        if (specificCard != null)
            return inputs.Count(c => c == specificCard) >= requiredCount;
        else
            return inputs.Count(c => c.cardType == cardType) >= requiredCount;
    }
}

/// <summary>
/// 定义组合的结果（支持多个 & 概率）
/// </summary>
[System.Serializable]
public class ResultCard
{
    public CardsBasicData resultCard;
    [Range(1, 10)]
    public int quantity = 1;
    [Range(0f, 1f)]
    public float probability = 1f;
}

/// <summary>
/// 定义一条组合规则
/// </summary>
[System.Serializable]
public class CardsCombinationRule
{
    public string combinationName;

    [Tooltip("本规则所需的所有条件（支持多张/同类匹配）")]
    public List<RequiredCardGroup> requiredCards = new List<RequiredCardGroup>();

    [Tooltip("可能产出的卡牌（支持概率/数量）")]
    public List<ResultCard> results = new List<ResultCard>();

    [Tooltip("合成耗时（秒）")]
    public float time = 0f;
}

/// <summary>
/// 数据库 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CardsCombination", menuName = "AntGame/Cards Combination Data")]
public class CardsCombination : ScriptableObject
{
    [Header("卡牌组合规则")]
    public List<CardsCombinationRule> combinations = new List<CardsCombinationRule>();

    public CardsCombinationRule GetCombination(List<CardsBasicData> inputCards)
    {
        foreach (var combo in combinations)
        {
            bool match = true;
            foreach (var req in combo.requiredCards)
            {
                if (!req.Matches(inputCards))
                {
                    match = false;
                    break;
                }
            }
            if (match) return combo;
        }
        return null;
    }
}
