using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ResourceCardData.cs
[CreateAssetMenu(fileName = "NewResourceCard", menuName = "Cards/RealData/resource")]
public class ResourceBasicData : CardsBasicData
{
    [Header("资源属性")]
    public ResourceType resourceType;
    public enum ResourceType { Leaf, Fungus }

    public int resourceValue = 1;
    public float decayRate = 0.1f; // 腐败速率
}