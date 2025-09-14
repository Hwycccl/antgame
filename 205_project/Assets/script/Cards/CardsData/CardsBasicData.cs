// CardsBasicData.cs 
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/RealData/basicCard")]
public class CardsBasicData : ScriptableObject
{
    [Header("���A����")]
    public string cardName = "�¿���";
    public Sprite cardImage;
    [TextArea(3, 5)] public string description;

    // --- �޸��c �_ʼ ---
    [Header("�����A���w")]
    [Tooltip("�����@�����Ɣ������[������A���w")]
    public GameObject cardPrefab; // ÿ�����Ɣ����F��ֱ���������Լ����A�Ƽ�
    // --- �޸��c �Y�� ---

    [Header("�������")]
    public CardType cardType;
    public enum CardType { Ant, Resource, Building, Effect ,Enemy}

    [Header("�ѯB/����")]
    public bool isConsumable = false;
    public bool isStackable = true;
    public int stackLimit = 10;

    [Header("�YԴ/��ֵ")]
    public int leafCost = 0;
    public int fungusCost = 0;
    public int fertilizer = 0;
    public int health = 0;
    public int attack = 0;
}