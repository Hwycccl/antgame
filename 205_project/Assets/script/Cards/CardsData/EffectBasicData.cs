using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EffectCardData.cs

[CreateAssetMenu(fileName = "NewEffectCard", menuName = "Cards/RealData/effect")]
public class EffectBasicData : CardsBasicData
{
    [Header("Ч������")]
    public EffectType effectType;
    public enum EffectType { Buff, Debuff, Weather, Instant, Transform }

    [Header("������Ч������")]
    public int duration = 1;           // �����غ���
    public int intensity = 1;          // ǿ��/����
    public bool affectAll = false;     // �Ƿ�Ӱ��ȫ��

    [Header("����Ч������")]
    public AntTransformType transformType;
    public enum AntTransformType { None, ToSoldier, ToWorkerMini, ToWorkerSmall, ToWorkerMedium, ToScout }

    public Sprite effectIcon;          // ͼ�꣨����СҶ�ӡ���Ҷ�ӡ���Ҷ�ӡ��۾���
}
