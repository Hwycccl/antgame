using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LootItem
{
    public CardsBasicData card; // ���ܵ���Ŀ���
    [Range(0.01f, 100f)]
    public float weight = 1f;   // ����ę��أ���ֵԽ�󣬙C��Խ�ߣ�
}

[CreateAssetMenu(fileName = "NewLootTable", menuName = "AntGame/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootItem> items;

    // ���������S�C����һ�����Ɣ���
    public CardsBasicData GetRandomItem()
    {
        if (items == null || items.Count == 0) return null;

        float totalWeight = items.Sum(item => item.weight);
        float randomPoint = Random.Range(0, totalWeight);

        foreach (var item in items)
        {
            if (randomPoint < item.weight)
            {
                return item.card;
            }
            randomPoint -= item.weight;
        }
        return items[items.Count - 1].card; // ������ã���Փ�ϲ������е�
    }
}
