using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EffectCardData.cs

[CreateAssetMenu(fileName = "NewEffectCard", menuName = "Cards/RealData/effect")]
public class EffectBasicData : CardsBasicData
{
    [Header("效果属性")]
    public EffectType effectType;
    public enum EffectType { Buff, Debuff, Weather, Instant }

    public int duration = 1;           // 持续回合数
    public int intensity = 1;          // 强度/倍率
    public bool affectAll = false;     // 是否影响全场
}
