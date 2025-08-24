//ResourceBasicData
using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceCard", menuName = "Cards/RealData/resource")]
public class ResourceBasicData : CardsBasicData
{
    [Header("资源属性")]
    public ResourceType resourceType;
    public enum ResourceType { Leaf, LeafFragment, Feces, Fungus, Contamination }

    public int resourceValue = 1;
    public float decayRate = 0f;   // 腐败速率 (0 = 不腐败)
}
