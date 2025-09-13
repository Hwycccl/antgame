// 文件名: CardPool.cs
using UnityEngine;
using System.Collections.Generic;

public class CardPool : MonoBehaviour
{
    public static CardPool Instance { get; private set; }

    // 用卡牌预制体的名字作为Key，来存储每种卡牌的池子
    private Dictionary<string, Queue<Card>> poolDictionary = new Dictionary<string, Queue<Card>>();

    private void Awake()
    {
        Instance = this;
    }

    // 预先创建一些卡牌放入池中（可以在游戏开始时调用）
    public void Prewarm(GameObject prefab, int count)
    {
        if (!poolDictionary.ContainsKey(prefab.name))
        {
            poolDictionary.Add(prefab.name, new Queue<Card>());
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            poolDictionary[prefab.name].Enqueue(obj.GetComponent<Card>());
        }
    }

    // 从池中获取一个卡牌
    public Card Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;
        if (!poolDictionary.ContainsKey(key) || poolDictionary[key].Count == 0)
        {
            // 如果池子空了或者不存在，就创建一个新的（作为备用方案）
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.name = key; // 保持名字一致
            return obj.GetComponent<Card>();
        }

        Card cardToSpawn = poolDictionary[key].Dequeue();
        cardToSpawn.gameObject.SetActive(true);
        cardToSpawn.transform.position = position;
        cardToSpawn.transform.rotation = rotation;

        return cardToSpawn;
    }

    // 将卡牌返还给池子
    public void Return(Card card)
    {
        if (card == null) return;

        string key = card.CardData.cardPrefab.name;
        if (!poolDictionary.ContainsKey(key))
        {
            // 如果这个类型的池子不存在，就创建一个
            poolDictionary.Add(key, new Queue<Card>());
        }

        card.gameObject.SetActive(false);
        poolDictionary[key].Enqueue(card);
    }
}