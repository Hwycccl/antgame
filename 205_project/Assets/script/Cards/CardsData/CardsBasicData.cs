//CardsBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/RealData/basicCard")]
public class CardsBasicData : ScriptableObject
{
    [Header("��������")]
    public string cardName = "�¿���";
    public Sprite cardImage;
    [TextArea(3, 5)] public string description;

    [Header("��������")]
    public CardType cardType;
    public enum CardType { Ant, Resource, Building, Effect }

    [Header("�ѵ�/����")]
    public bool isConsumable = false;   // ʹ�ú��Ƿ��Ƴ�
    public bool isStackable = true;     // �ܷ����
    public int stackLimit = 10;         // ���ѵ�����

    [Header("��Դ/��ֵ")]
    public int leafCost = 0;
    public int fungusCost = 0;
    public int fertilizer = 0;
    public int health = 0;
    public int attack = 0;
}
