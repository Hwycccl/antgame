using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LootItem
{
    public CardsBasicData card; // 可能掉落的卡牌
    [Range(0.01f, 100f)]
    public float weight = 1f;   // 掉落的權重（數值越大，機率越高）
}

[CreateAssetMenu(fileName = "NewLootTable", menuName = "AntGame/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootItem> items;

    // 根據權重隨機返回一個卡牌數據
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
        return items[items.Count - 1].card; // 作為備用，理論上不會執行到
    }
}
