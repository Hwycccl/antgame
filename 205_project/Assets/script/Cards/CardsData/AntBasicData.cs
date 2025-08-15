using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AntCardData.cs
[CreateAssetMenu(fileName = "NewAntCard", menuName = "Cards/RealData/ant")]
public class AntBasicData : CardsBasicData
{
    [Header("¬Ï“œ Ù–‘")]
    public AntType antType;
    public enum AntType { Worker, Soldier, Queen, Scout,larvae}

    public float workEfficiency = 1f;
}