// 文件路径: Assets/script/Cards/CardsData/AntNestData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Ant Nest Card", menuName = "Cards/Ant Nest Card")]
public class AntNestData : BuildingBasicData // 继承自建筑卡牌
{
    [Header("蚁穴设置")]
    [Tooltip("这个蚁穴能支持的最大蚂蚁数量")]
    public int populationCapacity = 10;
}