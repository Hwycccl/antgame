//AntBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewAntCard", menuName = "Cards/RealData/ant")]
public class AntBasicData : CardsBasicData
{
    [Header("蚂蚁属性")]
    public AntType antType;
    public enum AntType { WorkerMini, WorkerSmall, WorkerMedium, Soldier, Queen, Larva, Scout }

    public float workEfficiency = 1f;  // 采集/合成效率倍率
    public float growthTime = 0f;      // 从幼体进化到成体的时间
}
