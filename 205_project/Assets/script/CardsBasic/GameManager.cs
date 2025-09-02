// �����: GameManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("��ʼ�����O��")]
    [Tooltip("�[���_ʼ�r���ڈ������ɵĿ��Ɣ����б�")]
    [SerializeField] private List<CardsBasicData> startingCards;

    [Header("����λ��")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(-5f, 0, 0);
    [SerializeField] private float spawnSpacing = 2.0f; // ÿ������֮�g���g��

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SpawnInitialCards();
    }

    private void SpawnInitialCards()
    {
        if (startingCards == null || startingCards.Count == 0)
        {
            Debug.LogWarning("�]���O���κγ�ʼ���ƣ�");
            return;
        }

        for (int i = 0; i < startingCards.Count; i++)
        {
            Vector3 position = spawnPosition + new Vector3(i * spawnSpacing, 0, 0);
            CardSpawner.Instance.SpawnCard(startingCards[i], position);
        }

        Debug.Log($"�ɹ����� {startingCards.Count} ����ʼ���ơ�");
    }
}