// 放置於: ScoutingZone.cs (請完整複製此檔案的全部內容)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoutingZone : MonoBehaviour
{
    // --- 變數定義區 (錯誤的根源就在於遺漏了這一段) ---
    [Header("偵察設置")]
    [Tooltip("基礎偵察間隔（秒），偵察蟻越多間隔越短")]
    [SerializeField] private float baseScoutInterval = 20f;
    [Tooltip("每隻偵察蟻能減少多少秒的間隔")]
    [SerializeField] private float intervalReductionPerScout = 1.5f;
    [Tooltip("偵察間隔最短不能低於多少秒")]
    [SerializeField] private float minScoutInterval = 3f;

    [Header("事件機率")]
    [Range(0f, 1f)]
    [Tooltip("觸發負面事件（如寄生蠅）的基礎機率")]
    [SerializeField] private float hostileEventChance = 0.15f;

    [Header("數據關聯")]
    [Tooltip("將你創建的 ScoutingLootTable 拖到這裡")]
    [SerializeField] private LootTable discoveryLootTable;
    [Tooltip("寄生蠅卡牌的數據")]
    [SerializeField] private CardsBasicData parasitoidFlyCard;
    [Tooltip("污染卡牌的數據")]
    [SerializeField] private CardsBasicData contaminationCard;
    [Tooltip("偵察蟻卡牌的數據，用於識別")]
    [SerializeField] private CardsBasicData scoutAntCardData; // <--- scoutAntData 在這裡被定義
    [Tooltip("花園卡牌的數據，用於污染事件")]
    [SerializeField] private CardsBasicData gardenCardData;

    [Tooltip("偵察蟻卡牌的數據，用於識別")]
    [SerializeField] private CardsBasicData scoutAntData;

    [Header("位置設置")]
    [Tooltip("發現的卡牌生成位置的參考點")]
    [SerializeField] private Transform spawnPoint;

    private List<Card> scoutsInZone = new List<Card>();
    private float currentScoutTimer = 0f;
    // --- 變數定義區 結束 ---

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
        Debug.Log($"偵察事件觸發！當前有 {scoutsInZone.Count} 隻偵察蟻。");

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
            Debug.Log("負面事件：寄生蠅出現！");
            CardSpawner.Instance.SpawnCard(parasitoidFlyCard, spawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0, 0));
        }
        else
        {
            var gardens = FindObjectsByType<Card>(FindObjectsSortMode.None).Where(c => c.CardData == gardenCardData).ToList();
            if (gardens.Count > 0)
            {
                Debug.Log("負面事件：花園被污染！");
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
        Debug.Log("發現事件：找到了新東西！");
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
        if (other.TryGetComponent<Card>(out Card card) && card.CardData == scoutAntData) // <-- 現在這裡可以正確找到 scoutAntData
        {
            if (!scoutsInZone.Contains(card))
            {
                scoutsInZone.Add(card);
                Debug.Log($"一隻偵察蟻 [{card.name}] 進入了區域。當前數量: {scoutsInZone.Count}");
                if (scoutsInZone.Count == 1) ResetTimer();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Card>(out Card card) && card.CardData == scoutAntData) // <-- 現在這裡可以正確找到 scoutAntData
        {
            if (scoutsInZone.Remove(card))
            {
                Debug.Log($"一隻偵察蟻 [{card.name}] 離開了區域。當前數量: {scoutsInZone.Count}");
                if (scoutsInZone.Count == 0) currentScoutTimer = 0;
            }
        }
    }
}