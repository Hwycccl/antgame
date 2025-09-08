// 放置於: ScoutingZone.cs (最终完整版)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 这个数据结构保持不变
[System.Serializable]
public class ScoutAreaData
{
    public string areaName;
    public Sprite areaImage;
    public LootTable lootTable;
}

public class ScoutingZone : MonoBehaviour
{
    [Header("偵察設置")]
    [SerializeField] private float baseScoutInterval = 20f;
    [SerializeField] private float intervalReductionPerScout = 1.5f;
    [SerializeField] private float minScoutInterval = 3f;

    [Header("事件機率")]
    [Range(0f, 1f)]
    [SerializeField] private float hostileEventChance = 0.15f;

    [Header("數據關聯")]
    public List<ScoutAreaData> availableScoutAreas;
    [Tooltip("将列表中的一个区域作为默认侦察区域")]
    [SerializeField] private ScoutAreaData defaultScoutArea;
    private LootTable currentLootTable;

    [Tooltip("寄生蠅卡牌的數據")]
    [SerializeField] private CardsBasicData parasitoidFlyCard;
    [Tooltip("污染卡牌的數據")]
    [SerializeField] private CardsBasicData contaminationCard;
    [Tooltip("偵察蟻卡牌的數據，用於識別")]
    [SerializeField] private CardsBasicData scoutAntCardData;
    [Tooltip("花園卡牌的數據，用於污染事件")]
    [SerializeField] private CardsBasicData gardenCardData;

    [Header("位置設置")]
    [SerializeField] private Transform spawnPoint;

    private List<Card> scoutsInZone = new List<Card>();
    private float currentScoutTimer = 0f;

    private void Start()
    {
        // 设置默认掉落表
        if (defaultScoutArea != null && defaultScoutArea.lootTable != null)
        {
            currentLootTable = defaultScoutArea.lootTable;
        }
        else if (availableScoutAreas.Count > 0)
        {
            currentLootTable = availableScoutAreas[0].lootTable;
        }
    }

    // 公共方法，给UI按钮调用来切换掉落表
    public void SetScoutingArea(LootTable selectedLootTable)
    {
        if (selectedLootTable != null)
        {
            currentLootTable = selectedLootTable;
            Debug.Log($"侦察区域已切换为: {currentLootTable.name}");
        }
    }

    // 侦察逻辑
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
                Debug.Log($"一隻偵察蟻 [{enteredCard.name}] 進入了區域。當前數量: {scoutsInZone.Count}");

                // --- 核心修改：命令进入的侦察蚁保持提升状态 ---
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
                Debug.Log($"一隻偵察蟻 [{exitedCard.name}] 離開了區域。當前數量: {scoutsInZone.Count}");

                // --- 核心修改：命令离开的侦察蚁恢复原始状态 ---
                if (exitedCard.TryGetComponent<ElevationController>(out var elevation))
                {
                    elevation.SetElevated(false);
                }

                if (scoutsInZone.Count == 0) currentScoutTimer = 0;
            }
        }
    }

    // --- 其他辅助函数 ---
    private void ResetTimer()
    {
        float interval = baseScoutInterval - (scoutsInZone.Count * intervalReductionPerScout);
        currentScoutTimer = Mathf.Max(interval, minScoutInterval);
    }

    private void TriggerScoutEvent()
    {
        if (currentLootTable == null) return;
        Debug.Log($"偵察事件觸發！當前有 {scoutsInZone.Count} 隻偵察蟻。");

        if (Random.value < hostileEventChance) TriggerHostileEvent();
        else TriggerDiscoveryEvent();
    }

    private void TriggerDiscoveryEvent()
    {
        Debug.Log($"发现事件：在 [{currentLootTable.name}] 找到了新东西！");
        CardsBasicData foundCardData = currentLootTable.GetRandomItem();
        if (foundCardData != null)
        {
            CardSpawner.Instance.SpawnCard(foundCardData, spawnPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0));
        }
    }

    // --- 完整的敌对事件和协程函数 ---
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
}