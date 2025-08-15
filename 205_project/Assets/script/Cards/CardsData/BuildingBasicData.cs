using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BuildingCardData.cs
[CreateAssetMenu(fileName = "NewBuildingCard", menuName = "Cards/RealData/building")]
public class BuildingBasicData : CardsBasicData
{
    [Header("建筑属性")]
    public BuildingType buildingType;
    public enum BuildingType { Nest, Storage, Barracks, Garden }

    public int maxHp = 5;              // 建筑耐久
    public int productionPerTurn = 1;  // 每回合产出
    public int capacity = 10;          // 容量上限
}
