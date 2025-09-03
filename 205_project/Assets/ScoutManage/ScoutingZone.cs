// �����: ScoutingZone.cs (Ո�����}�u�˙n����ȫ������)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoutingZone : MonoBehaviour
{
    // --- ׃�����x�^ (�e�`�ĸ�Դ������z©���@һ��) ---
    [Header("�ɲ��O��")]
    [Tooltip("���A�ɲ��g�����룩���ɲ�ρԽ���g��Խ��")]
    [SerializeField] private float baseScoutInterval = 20f;
    [Tooltip("ÿ�b�ɲ�ρ�ܜp�ٶ�������g��")]
    [SerializeField] private float intervalReductionPerScout = 1.5f;
    [Tooltip("�ɲ��g����̲��ܵ�춶�����")]
    [SerializeField] private float minScoutInterval = 3f;

    [Header("�¼��C��")]
    [Range(0f, 1f)]
    [Tooltip("�|�lؓ���¼��������ω���Ļ��A�C��")]
    [SerializeField] private float hostileEventChance = 0.15f;

    [Header("�����P")]
    [Tooltip("���ㄓ���� ScoutingLootTable �ϵ��@�e")]
    [SerializeField] private LootTable discoveryLootTable;
    [Tooltip("����ω���ƵĔ���")]
    [SerializeField] private CardsBasicData parasitoidFlyCard;
    [Tooltip("��Ⱦ���ƵĔ���")]
    [SerializeField] private CardsBasicData contaminationCard;
    [Tooltip("�ɲ�ρ���ƵĔ���������R�e")]
    [SerializeField] private CardsBasicData scoutAntCardData; // <--- scoutAntData ���@�e�����x
    [Tooltip("���@���ƵĔ����������Ⱦ�¼�")]
    [SerializeField] private CardsBasicData gardenCardData;

    [Tooltip("�ɲ�ρ���ƵĔ���������R�e")]
    [SerializeField] private CardsBasicData scoutAntData;

    [Header("λ���O��")]
    [Tooltip("�l�F�Ŀ�������λ�õą����c")]
    [SerializeField] private Transform spawnPoint;

    private List<Card> scoutsInZone = new List<Card>();
    private float currentScoutTimer = 0f;
    // --- ׃�����x�^ �Y�� ---

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

    private void TriggerScoutEvent()
    {
        Debug.Log($"�ɲ��¼��|�l����ǰ�� {scoutsInZone.Count} �b�ɲ�ρ��");

        if (Random.value < hostileEventChance)
        {
            TriggerHostileEvent();
        }
        else
        {
            TriggerDiscoveryEvent();
        }
    }

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

    private void TriggerDiscoveryEvent()
    {
        Debug.Log("�l�F�¼����ҵ����|����");
        CardsBasicData foundCardData = discoveryLootTable.GetRandomItem();
        if (foundCardData != null)
        {
            CardSpawner.Instance.SpawnCard(foundCardData, spawnPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0));
        }
    }

    private void ResetTimer()
    {
        float interval = baseScoutInterval - (scoutsInZone.Count * intervalReductionPerScout);
        currentScoutTimer = Mathf.Max(interval, minScoutInterval);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Card>(out Card card) && card.CardData == scoutAntData) // <-- �F���@�e�������_�ҵ� scoutAntData
        {
            if (!scoutsInZone.Contains(card))
            {
                scoutsInZone.Add(card);
                Debug.Log($"һ�b�ɲ�ρ [{card.name}] �M���˅^�򡣮�ǰ����: {scoutsInZone.Count}");
                if (scoutsInZone.Count == 1) ResetTimer();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Card>(out Card card) && card.CardData == scoutAntData) // <-- �F���@�e�������_�ҵ� scoutAntData
        {
            if (scoutsInZone.Remove(card))
            {
                Debug.Log($"һ�b�ɲ�ρ [{card.name}] �x�_�˅^�򡣮�ǰ����: {scoutsInZone.Count}");
                if (scoutsInZone.Count == 0) currentScoutTimer = 0;
            }
        }
    }
}