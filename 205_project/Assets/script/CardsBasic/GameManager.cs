//
// GameManager.cs (�޸���)
//
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("��ʼ����")]
    [Tooltip("�[���_ʼ�r��ֱ���ڈ������ɵĿ���")]
    [SerializeField] private List<CardsBasicData> startingCards = new List<CardsBasicData>();

    [Header("��ʼ���������O��")]
    [SerializeField] private Vector3 startPosition = new Vector3(0, 0, 0);
    [SerializeField] private float initialSpawnSpacing = 2.0f; // ÿ����ʼ����֮�g���g��

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // �[���_ʼ�r������������ʼ����
        for (int i = 0; i < startingCards.Count; i++)
        {
            var cardData = startingCards[i];
            if (cardData != null)
            {
                // Ӌ��ÿ����ʼ���Ƶ�λ��
                Vector3 spawnPos = startPosition + new Vector3(i * initialSpawnSpacing, 0, 0);

                // ͬ�r֪ͨ߉݋��UI
                CardsManager.Instance.AddCardToLogic(cardData);
                HandUI.Instance.AddCardToView(cardData, spawnPos);
            }
        }

        Debug.Log("�[���_ʼ�������� " + startingCards.Count + " ����ʼ����");
    }
}