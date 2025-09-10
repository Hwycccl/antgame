// 放置于: SurvivalManager.cs (English Version)
using UnityEngine;
using System.Linq;

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
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        int antPopulation = allCards.Count(c => c.CardData.cardType == CardsBasicData.CardType.Ant);
        int totalFoodAvailable = allCards
            .Where(c => c.CardData == foodCardData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        int foodNeeded = antPopulation * foodConsumptionPerAnt;

        Debug.Log($"[Food Check] Population: {antPopulation}, Food Needed: {foodNeeded}, Food Available: {totalFoodAvailable}");

        if (totalFoodAvailable < foodNeeded)
        {
            // 使用英文的游戏结束原因
            GameOver($"Your colony perished from starvation!\nIt needed {foodNeeded} food but only had {totalFoodAvailable}.");
        }
        else
        {
            ConsumeFood(foodNeeded, allCards.Where(c => c.CardData == foodCardData).ToList());
        }
    }

    private void ConsumeFood(int amountToConsume, System.Collections.Generic.List<Card> foodCards)
    {
        foodCards = foodCards.OrderBy(c => (c.CardData as ResourceBasicData).resourceValue).ToList();
        int consumedAmount = 0;
        foreach (var foodCard in foodCards)
        {
            if (consumedAmount >= amountToConsume) break;
            int cardValue = (foodCard.CardData as ResourceBasicData).resourceValue;
            consumedAmount += cardValue;
            Destroy(foodCard.gameObject);
        }
        Debug.Log($"Consumed {consumedAmount} food.");
    }

    private void CheckInvasion()
    {
        invasionCount++;
        int requiredSoldiers = baseSoldiersRequired + (invasionCount - 1) * soldiersRequiredIncrease;

        Debug.Log($"[Invasion Alert!] Wave {invasionCount} is attacking! {requiredSoldiers} soldiers are needed to defend.");

        int currentSoldiers = FindObjectsByType<Card>(FindObjectsSortMode.None)
            .Count(c => c.CardData == soldierAntCardData);

        Debug.Log($"Current soldier count: {currentSoldiers}");

        if (currentSoldiers < requiredSoldiers)
        {
            // 使用英文的游戏结束原因
            GameOver($"Your colony was overrun by invaders!\nYou needed {requiredSoldiers} soldiers but only had {currentSoldiers}.");
        }
        else
        {
            Debug.Log("The colony successfully defended against the invasion!");
        }
    }

    private void GameOver(string reason)
    {
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