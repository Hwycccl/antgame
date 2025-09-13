// �ļ���: CardPool.cs
using UnityEngine;
using System.Collections.Generic;

public class CardPool : MonoBehaviour
{
    public static CardPool Instance { get; private set; }

    // �ÿ���Ԥ�����������ΪKey�����洢ÿ�ֿ��Ƶĳ���
    private Dictionary<string, Queue<Card>> poolDictionary = new Dictionary<string, Queue<Card>>();

    private void Awake()
    {
        Instance = this;
    }

    // Ԥ�ȴ���һЩ���Ʒ�����У���������Ϸ��ʼʱ���ã�
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

    // �ӳ��л�ȡһ������
    public Card Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;
        if (!poolDictionary.ContainsKey(key) || poolDictionary[key].Count == 0)
        {
            // ������ӿ��˻��߲����ڣ��ʹ���һ���µģ���Ϊ���÷�����
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.name = key; // ��������һ��
            return obj.GetComponent<Card>();
        }

        Card cardToSpawn = poolDictionary[key].Dequeue();
        cardToSpawn.gameObject.SetActive(true);
        cardToSpawn.transform.position = position;
        cardToSpawn.transform.rotation = rotation;

        return cardToSpawn;
    }

    // �����Ʒ���������
    public void Return(Card card)
    {
        if (card == null) return;

        string key = card.CardData.cardPrefab.name;
        if (!poolDictionary.ContainsKey(key))
        {
            // ���������͵ĳ��Ӳ����ڣ��ʹ���һ��
            poolDictionary.Add(key, new Queue<Card>());
        }

        card.gameObject.SetActive(false);
        poolDictionary[key].Enqueue(card);
    }
}