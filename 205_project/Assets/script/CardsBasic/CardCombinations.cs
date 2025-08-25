using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ������������һ�࿨�ƣ���ָ�����忨 or �������ͣ�
/// </summary>
[System.Serializable]
public class RequiredCardGroup
{
    [Tooltip("ָ����Ҫ�ľ��忨�����ݣ����Ϊ��������ƥ��")]
    public CardsBasicData specificCard;

    [Tooltip("��Ҫ�Ŀ������ͣ���� specificCard ��Ϊ������ԣ�")]
    public CardsBasicData.CardType cardType;

    [Tooltip("��Ҫ�Ŀ���������>=1��")]
    public int requiredCount = 1;

    [Tooltip("�ϳ�ʱ�Ƿ�������Щ����")]
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
/// ������ϵĽ����֧�ֶ�� & ���ʣ�
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
/// ����һ����Ϲ���
/// </summary>
[System.Serializable]
public class CardsCombinationRule
{
    public string combinationName;

    [Tooltip("���������������������֧�ֶ���/ͬ��ƥ�䣩")]
    public List<RequiredCardGroup> requiredCards = new List<RequiredCardGroup>();

    [Tooltip("���ܲ����Ŀ��ƣ�֧�ָ���/������")]
    public List<ResultCard> results = new List<ResultCard>();

    [Tooltip("�ϳɺ�ʱ���룩")]
    public float time = 0f;
}

/// <summary>
/// ���ݿ� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CardsCombination", menuName = "AntGame/Cards Combination Data")]
public class CardsCombination : ScriptableObject
{
    [Header("������Ϲ���")]
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
