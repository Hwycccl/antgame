// �����: ScoutingZone.cs (����������)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ������ݽṹ���ֲ���
[System.Serializable]
public class ScoutAreaData
{
    public string areaName;
    public Sprite areaImage;
    public LootTable lootTable;
}

public class ScoutingZone : MonoBehaviour
{
    [Header("�ɲ��O��")]
    [SerializeField] private float baseScoutInterval = 20f;
    [SerializeField] private float intervalReductionPerScout = 1.5f;
    [SerializeField] private float minScoutInterval = 3f;

    [Header("�¼��C��")]
    [Range(0f, 1f)]
    [SerializeField] private float hostileEventChance = 0.15f;

    [Header("�����P")]
    public List<ScoutAreaData> availableScoutAreas;
    [Tooltip("���б��е�һ��������ΪĬ���������")]
    [SerializeField] private ScoutAreaData defaultScoutArea;
    private LootTable currentLootTable;

    [Tooltip("����ω���ƵĔ���")]
    [SerializeField] private CardsBasicData parasitoidFlyCard;
    [Tooltip("��Ⱦ���ƵĔ���")]
    [SerializeField] private CardsBasicData contaminationCard;
    [Tooltip("�ɲ�ρ���ƵĔ���������R�e")]
    [SerializeField] private CardsBasicData scoutAntCardData;
    [Tooltip("���@���ƵĔ����������Ⱦ�¼�")]
    [SerializeField] private CardsBasicData gardenCardData;

    [Header("λ���O��")]
    [SerializeField] private Transform spawnPoint;

    private List<Card> scoutsInZone = new List<Card>();
    private float currentScoutTimer = 0f;

    private void Start()
    {
        // ����Ĭ�ϵ����
        if (defaultScoutArea != null && defaultScoutArea.lootTable != null)
        {
            currentLootTable = defaultScoutArea.lootTable;
        }
        else if (availableScoutAreas.Count > 0)
        {
            currentLootTable = availableScoutAreas[0].lootTable;
        }
    }

    // ������������UI��ť�������л������
    public void SetScoutingArea(LootTable selectedLootTable)
    {
        if (selectedLootTable != null)
        {
            currentLootTable = selectedLootTable;
            Debug.Log($"����������л�Ϊ: {currentLootTable.name}");
        }
    }

    // ����߼�
    private void Update()
    {
        if (scoutsInZone.Count > 0)
        {
            currentScoutTimer -= Time.deltaTime;
            if (currentScoutTimer <= 0)
            {
                TriggerScoutEvent();
                ResetTimer();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Card>(out Card enteredCard) && enteredCard.CardData == scoutAntCardData)
        {
            if (!scoutsInZone.Contains(enteredCard))
            {
                scoutsInZone.Add(enteredCard);
                Debug.Log($"һ�b�ɲ�ρ [{enteredCard.name}] �M���˅^�򡣮�ǰ����: {scoutsInZone.Count}");

                // --- �����޸ģ�������������ϱ�������״̬ ---
                if (enteredCard.TryGetComponent<ElevationController>(out var elevation))
                {
                    elevation.SetElevated(true);
                }

                if (scoutsInZone.Count == 1) ResetTimer();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Card>(out Card exitedCard) && exitedCard.CardData == scoutAntCardData)
        {
            if (scoutsInZone.Remove(exitedCard))
            {
                Debug.Log($"һ�b�ɲ�ρ [{exitedCard.name}] �x�_�˅^�򡣮�ǰ����: {scoutsInZone.Count}");

                // --- �����޸ģ������뿪������ϻָ�ԭʼ״̬ ---
                if (exitedCard.TryGetComponent<ElevationController>(out var elevation))
                {
                    elevation.SetElevated(false);
                }

                if (scoutsInZone.Count == 0) currentScoutTimer = 0;
            }
        }
    }

    // --- ������������ ---
    private void ResetTimer()
    {
        float interval = baseScoutInterval - (scoutsInZone.Count * intervalReductionPerScout);
        currentScoutTimer = Mathf.Max(interval, minScoutInterval);
    }

    private void TriggerScoutEvent()
    {
        if (currentLootTable == null) return;
        Debug.Log($"�ɲ��¼��|�l����ǰ�� {scoutsInZone.Count} �b�ɲ�ρ��");

        if (Random.value < hostileEventChance) TriggerHostileEvent();
        else TriggerDiscoveryEvent();
    }

    private void TriggerDiscoveryEvent()
    {
        Debug.Log($"�����¼����� [{currentLootTable.name}] �ҵ����¶�����");
        CardsBasicData foundCardData = currentLootTable.GetRandomItem();
        if (foundCardData != null)
        {
            CardSpawner.Instance.SpawnCard(foundCardData, spawnPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0));
        }
    }

    // --- �����ĵж��¼���Э�̺��� ---
    private void TriggerHostileEvent()
    {
        if (Random.value < 0.5f)
        {
            Debug.Log("ؓ���¼�������ω���F��");
            CardSpawner.Instance.SpawnCard(parasitoidFlyCard, spawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0, 0));
        }
        else
        {
            var gardens = FindObjectsByType<Card>(FindObjectsSortMode.None).Where(c => c.CardData == gardenCardData).ToList();
            if (gardens.Count > 0)
            {
                Debug.Log("ؓ���¼������@����Ⱦ��");
                Card targetGarden = gardens[Random.Range(0, gardens.Count)];
                Card contamination = CardSpawner.Instance.SpawnCard(contaminationCard, targetGarden.transform.position);

                if (contamination != null)
                {
                    StartCoroutine(ForceStack(contamination, targetGarden));
                }
            }
        }
    }

    private IEnumerator ForceStack(Card cardToStack, Card destinationCard)
    {
        yield return new WaitForSeconds(0.1f);
        if (cardToStack != null && destinationCard != null)
        {
            cardToStack.Stacker.ForceStackOn(destinationCard.Stacker);
        }
    }
}