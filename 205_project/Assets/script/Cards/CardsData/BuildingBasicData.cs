using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BuildingCardData.cs
[CreateAssetMenu(fileName = "NewBuildingCard", menuName = "Cards/RealData/building")]
public class BuildingBasicData : CardsBasicData
{
    [Header("��������")]
    public BuildingType buildingType;
    public enum BuildingType { Nest, Storage, Barracks, Garden }

    public int maxHp = 5;              // �����;�
    public int productionPerTurn = 1;  // ÿ�غϲ���
    public int capacity = 10;          // ��������
}
