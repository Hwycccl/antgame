//BuildBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingCard", menuName = "Cards/RealData/building")]
public class BuildingBasicData : CardsBasicData
{
    [Header("建筑属性")]
    public BuildingType buildingType;
    public enum BuildingType { QueenChamber, Hatchery, Storage, Garden, Dump ,AntNet}

    public int maxHp = 10;              // 建筑耐久
    public int productionPerTurn = 0;   // 每回合产出
    public int capacity = 10;           // 容量上限
}
