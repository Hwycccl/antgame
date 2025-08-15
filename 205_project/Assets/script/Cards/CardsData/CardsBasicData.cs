using System.Collections;
using System.Collections.Generic;
// CardData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/RealData/basicCard")]//�½�һ�������ļ�
public class CardsBasicData : ScriptableObject//���Ա�����ط����ã��ڴ���ֻ��һ�ݣ�ʡ��Դ
{
    [Header("��������")]
    public string cardName = "�¿���";
    public Sprite cardImage;
    [TextArea(3, 5)] public string description;

    [Header("��������")]
    public CardType cardType;
    public enum CardType { Ant, Resource, Building, Effect }

    [Header("��Դ����")]
    public int leafCost = 0;
    public int fungusCost = 0;
    public int fertilizer = 0;

    [Header("��������")]
    public int health;
    public int attack;

    [Header("��Ϊ����")]
    public bool isConsumable = false;//ʹ�ú��Ƿ��Ƴ�
    public bool isStackable = true;//�ܷ��ص�
}