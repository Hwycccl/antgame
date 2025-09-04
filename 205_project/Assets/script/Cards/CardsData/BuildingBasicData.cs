//BuildBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingCard", menuName = "Cards/RealData/building")]
public class BuildingBasicData : CardsBasicData
{
    [Header("��������")]
    public BuildingType buildingType;
    public enum BuildingType { QueenChamber, Hatchery, Storage, Garden, Dump ,AntNet}

    public int maxHp = 10;              // �����;�
    public int productionPerTurn = 0;   // ÿ�غϲ���
    public int capacity = 10;           // ��������
}
