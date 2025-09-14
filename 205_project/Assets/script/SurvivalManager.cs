// 文件路径: Assets/script/SurvivalManager.cs (已按您的要求修改)
using UnityEngine;
using System.Linq;
using System.Collections.Generic; // 引入此命名空间以使用 List<T>

public class SurvivalManager : MonoBehaviour
{
    public static SurvivalManager Instance { get; private set; }

    [Header("Day/Night & Time Settings")]
    [Tooltip("How many real-world seconds correspond to one in-game day")]
    [SerializeField] private float secondsPerDay = 60f;

    [Header("Food Consumption")]
    [Tooltip("How much food each ant consumes per day")]
    [SerializeField] private int foodConsumptionPerAnt = 1;
    [Tooltip("Drag your 'Food' card data (e.g., 'Fungus') here")]
    [SerializeField] private ResourceBasicData foodCardData;

    [Header("Invasion Events")]
    [Tooltip("The first invasion occurs on which day?")]
    [SerializeField] private int firstInvasionDay = 5;
    [Tooltip("How many days between each invasion")]
    [SerializeField] private int invasionIntervalDays = 5;
    [Tooltip("Base number of soldiers required for the first invasion")]
    [SerializeField] private int baseSoldiersRequired = 3;
    [Tooltip("How many more soldiers are required for each subsequent invasion")]
    [SerializeField] private int soldiersRequiredIncrease = 2;
    [Tooltip("Drag your 'Soldier Ant' card data here")]
    [SerializeField] private AntBasicData soldierAntCardData;

    // --- 变量声明部分保持不变 ---
    private int currentDay = 1;
    private float dayTimer = 0f;
    private int nextInvasionDay;
    private int invasionCount = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.StartOrResumeTimer();
        }
        else
        {
            Debug.LogError("GlobalTimeManager not found in the scene!");
            this.enabled = false;
            return;
        }

        nextInvasionDay = firstInvasionDay;
        Debug.Log("Survival Manager has started.");
    }

    private void Update()
    {
        if (isGameOver || GlobalTimeManager.Instance.IsPaused) return;

        dayTimer += Time.deltaTime;

        if (dayTimer >= secondsPerDay)
        {
            dayTimer -= secondsPerDay;
            currentDay++;
            Debug.Log($"--- Day {currentDay} has begun ---");

            CheckFoodSupply();
            if (isGameOver) return;

            if (currentDay >= nextInvasionDay)
            {
                CheckInvasion();
                nextInvasionDay += invasionIntervalDays;
            }
        }
    }

    private void CheckFoodSupply()
    {
        // --- 这部分食物检查逻辑保持不变 ---
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        int antPopulation = allCards.Count(c => c.gameObject.activeInHierarchy && c.CardData.cardType == CardsBasicData.CardType.Ant);

        var availableFoodCards = allCards.Where(c => c.gameObject.activeInHierarchy && c.CardData == foodCardData).ToList();
        int totalFoodAvailable = availableFoodCards.Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        int foodNeeded = antPopulation * foodConsumptionPerAnt;

        Debug.Log($"[Food Check] Population: {antPopulation}, Food Needed: {foodNeeded}, Food Available: {totalFoodAvailable}");

        if (totalFoodAvailable < foodNeeded)
        {
            GameOver($"Your colony perished from starvation!\nIt needed {foodNeeded} food but only had {totalFoodAvailable}.");
        }
        else
        {
            ConsumeFood(foodNeeded, availableFoodCards);
        }
    }

    // ==================== 【核心修改区域】 ====================
    /// <summary>
    /// 消耗指定数量的食物。
    /// 主要修改：将 Destroy() 替换为 CardPool.Instance.Return()，以实现对象池回收。
    /// </summary>
    private void ConsumeFood(int amountToConsume, List<Card> foodCards)
    {
        // 1. 优先消耗价值较低的食物卡（此逻辑保持不变）
        foodCards = foodCards.OrderBy(c => (c.CardData as ResourceBasicData).resourceValue).ToList();

        int consumedValue = 0;
        List<Card> cardsToReturnToPool = new List<Card>();

        // 2. 决定哪些卡牌需要被消耗
        foreach (var foodCard in foodCards)
        {
            if (consumedValue >= amountToConsume) break;

            if (foodCard.CardData is ResourceBasicData resourceData)
            {
                consumedValue += resourceData.resourceValue;
                cardsToReturnToPool.Add(foodCard);
            }
        }

        // 3. 【关键修改】统一将需要消耗的卡牌返回到对象池
        foreach (var card in cardsToReturnToPool)
        {
            if (CardPool.Instance != null)
            {
                // 使用对象池回收卡牌，而不是销毁它
                CardPool.Instance.Return(card);
            }
            else
            {
                // 如果找不到对象池，则执行原始的销毁操作作为后备方案
                Destroy(card.gameObject);
            }
        }

        Debug.Log($"Consumed {consumedValue} food value by returning {cardsToReturnToPool.Count} cards to the pool.");
    }
    // ========================================================


    private void CheckInvasion()
    {
        // --- 入侵检查逻辑保持不变 ---
        invasionCount++;
        int requiredSoldiers = baseSoldiersRequired + (invasionCount - 1) * soldiersRequiredIncrease;

        Debug.Log($"[Invasion Alert!] Wave {invasionCount} is attacking! {requiredSoldiers} soldiers are needed to defend.");

        int currentSoldiers = FindObjectsByType<Card>(FindObjectsSortMode.None)
            .Count(c => c.gameObject.activeInHierarchy && c.CardData == soldierAntCardData);

        Debug.Log($"Current soldier count: {currentSoldiers}");

        if (currentSoldiers < requiredSoldiers)
        {
            GameOver($"Your colony was overrun by invaders!\nYou needed {requiredSoldiers} soldiers but only had {currentSoldiers}.");
        }
        else
        {
            Debug.Log("The colony successfully defended against the invasion!");
        }
    }

    private void GameOver(string reason)
    {
        // --- 游戏结束逻辑保持不变 ---
        if (isGameOver) return;
        isGameOver = true;

        Debug.LogError("================ GAME OVER ================");
        Debug.LogError(reason);
        Debug.LogError("==========================================");

        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.PauseTimer();
        }

        if (GameOverUI.Instance != null)
        {
            int finalPopulation = FindObjectsByType<Card>(FindObjectsSortMode.None)
                                      .Count(c => c.CardData.cardType == CardsBasicData.CardType.Ant);

            GameOverUI.Instance.ShowGameOverScreen(currentDay, finalPopulation, reason);
        }
        else
        {
            Debug.LogError("GameOverUI instance not found in the scene! Quitting application as a fallback.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}