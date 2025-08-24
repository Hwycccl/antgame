// CardCombinationsData.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ��������жԵ��ű��迨�Ƶ�Ҫ��
/// </summary>
[System.Serializable]
public class RequiredCard
{
    [Tooltip("����Ŀ�������")]
    public CardsBasicData cardData;
    [Tooltip("��ϳɹ����Ƿ��������ſ���")]
    public bool destroyOnCombine = true;
}

/// <summary>
/// ���嵥��������ϵĹ���
/// </summary>
[System.Serializable]
public class CardCombination
{
    public string combinationName;

    [Tooltip("�����������ſ����Լ����ǵ�����ѡ��")]
    public List<RequiredCard> requiredCards = new List<RequiredCard>(2);

    [Tooltip("��ϳɹ���������ɵĶ��⿨��")]
    public List<CardsBasicData> resultingCards = new List<CardsBasicData>();

    [Tooltip("��������¿�������ĵȴ�ʱ�䣨�룩")]
    public float time = 0f;
}

/// <summary>
/// �洢���п�����ϵ����ݿ� (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "CardCombinations", menuName = "AntGame/Card Combinations Data")]
public class CardCombinationsData : ScriptableObject
{
    [Header("��������б�")]
    public List<CardCombination> combinations = new List<CardCombination>();

    /// <summary>
    /// ������ſ����Ƿ�ƥ��ĳ����ϣ�������������������� (�������߼�)
    /// </summary>
    public CardCombination GetCombination(CardsBasicData card1, CardsBasicData card2)
    {
        foreach (var combination in combinations)
        {
            if (combination.requiredCards.Count != 2) continue;

            // ��ȡ�����ж�������ֿ�������
            CardsBasicData required1 = combination.requiredCards[0].cardData;
            CardsBasicData required2 = combination.requiredCards[1].cardData;

            // ��鴫��Ŀ����Ƿ������ƥ�䣨����˳��ߵ���
            bool match = (card1 == required1 && card2 == required2) ||
                         (card1 == required2 && card2 == required1);

            if (match)
            {
                return combination;
            }
        }
        return null; // û���ҵ�ƥ����
    }
}