//AntBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewAntCard", menuName = "Cards/RealData/ant")]
public class AntBasicData : CardsBasicData
{
    [Header("��������")]
    public AntType antType;
    public enum AntType { WorkerMini, WorkerSmall, WorkerMedium, Soldier, Queen, Larva, Scout }

    public float workEfficiency = 1f;  // �ɼ�/�ϳ�Ч�ʱ���
    public float growthTime = 0f;      // ����������������ʱ��
}
