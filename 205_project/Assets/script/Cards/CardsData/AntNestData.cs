// �ļ�·��: Assets/script/Cards/CardsData/AntNestData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Ant Nest Card", menuName = "Cards/Ant Nest Card")]
public class AntNestData : BuildingBasicData // �̳��Խ�������
{
    [Header("��Ѩ����")]
    [Tooltip("�����Ѩ��֧�ֵ������������")]
    public int populationCapacity = 10;
}