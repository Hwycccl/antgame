// 放置于: SurvivalManager.cs (游戏结束功能强化版)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 负责处理游戏的生存挑战，包括食物消耗和周期性入侵。
/// </summary>
public class SurvivalManager : MonoBehaviour
{
    
    public static SurvivalManager Instance { get; private set; }

    [Header("昼夜与时间设置")]
    [Tooltip("游戏中一天对应现实世界的多少秒")]
    [SerializeField] private float secondsPerDay = 60f;

    [Header("食物消耗设置")]
    [Tooltip("每只蚂蚁每天消耗多少食物")]
    [SerializeField] private int foodConsumptionPerAnt = 1;
    [Tooltip("将你的'食物'卡牌数据（例如'真菌'）拖到这里")]
    [SerializeField] private ResourceBasicData foodCardData; // 用于识别食物卡

    [Header("入侵事件设置")]
    [Tooltip("第一次入侵发生在第几天？")]
    [SerializeField] private int firstInvasionDay = 5;
    [Tooltip("每隔多少天发生一次入侵")]
    [SerializeField] private int invasionIntervalDays = 5;
    [Tooltip("入侵所需的基础士兵数")]
    [SerializeField] private int baseSoldiersRequired = 3;
    [Tooltip("每次入侵后，额外需要增加多少士兵")]
    [SerializeField] private int soldiersRequiredIncrease = 2;
    [Tooltip("将你的'士兵蚁'卡牌数据拖到这里")]
    [SerializeField] private AntBasicData soldierAntCardData; // 用于识别士兵蚁

    // --- 内部状态变量 ---
    private int currentDay = 0;
    private float dayTimer = 0f;
    private int nextInvasionDay;
    private int invasionCount = 0;
    private bool isGameOver = false;

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
        // 确保全局时间管理器已经启动
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.StartOrResumeTimer();
        }
        else
        {
            Debug.LogError("场景中缺少 GlobalTimeManager!");
            this.enabled = false; // 如果没有时间管理器，则禁用此脚本
            return;
        }

        nextInvasionDay = firstInvasionDay;
        Debug.Log("生存管理器已启动。");
    }

    private void Update()
    {
        Debug.Log("SurvivalManager Update is running...");
        if (isGameOver || GlobalTimeManager.Instance.IsPaused)
        {
            return;
        }

        dayTimer += Time.deltaTime;

        // 一天结束
        if (dayTimer >= secondsPerDay)
        {
            dayTimer -= secondsPerDay;
            currentDay++;
            Debug.Log($"--- 第 {currentDay} 天结束 ---");

            // 1. 执行每日食物检查
            CheckFoodSupply();
            if (isGameOver) return; // 如果游戏因饥饿结束，则停止后续检查

            // 2. 检查今天是否是入侵日
            if (currentDay >= nextInvasionDay)
            {
                CheckInvasion();
                // 计算下一次入侵的日期
                nextInvasionDay += invasionIntervalDays;
            }
        }
    }

    /// <summary>
    /// 检查食物供应是否充足
    /// </summary>
    private void CheckFoodSupply()
    {
        // 找出场景中所有的蚂蚁和食物卡
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

        int antPopulation = allCards.Count(c => c.CardData.cardType == CardsBasicData.CardType.Ant);
        // 只计算食物卡的总资源值
        int totalFoodAvailable = allCards
            .Where(c => c.CardData == foodCardData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        int foodNeeded = antPopulation * foodConsumptionPerAnt;

        Debug.Log($"蚂蚁总数: {antPopulation}，需要食物: {foodNeeded}，当前食物储备: {totalFoodAvailable}");

        if (totalFoodAvailable < foodNeeded)
        {
            // 食物不足，游戏结束
            GameOver($"游戏结束：饥荒！\n你的蚁群需要 {foodNeeded} 食物，但只有 {totalFoodAvailable}。");
        }
        else
        {
            // 食物充足，消耗食物
            Debug.Log("食物充足，开始消耗...");
            ConsumeFood(foodNeeded, allCards);
        }
    }

    /// <summary>
    /// 从场景中移除指定数量的食物卡牌
    /// </summary>
    private void ConsumeFood(int amountToConsume, Card[] allCards)
    {
        // 筛选出所有食物卡牌，并按其资源价值从低到高排序，优先消耗价值低的
        List<Card> foodCards = allCards
            .Where(c => c.CardData == foodCardData)
            .OrderBy(c => (c.CardData as ResourceBasicData).resourceValue)
            .ToList();

        int consumedAmount = 0;
        foreach (var foodCard in foodCards)
        {
            if (consumedAmount >= amountToConsume)
                break;

            int cardValue = (foodCard.CardData as ResourceBasicData).resourceValue;
            consumedAmount += cardValue;

            Debug.Log($"消耗了卡牌 '{foodCard.name}' (价值: {cardValue})。");
            Destroy(foodCard.gameObject);
        }
        Debug.Log($"总共消耗了 {consumedAmount} 点食物。");
    }


    /// <summary>
    /// 检查入侵事件
    /// </summary>
    private void CheckInvasion()
    {
        invasionCount++;
        int requiredSoldiers = baseSoldiersRequired + (invasionCount - 1) * soldiersRequiredIncrease;

        Debug.Log($"!!! 入侵警报 (第 {invasionCount} 波) !!! 需要 {requiredSoldiers} 名士兵蚁进行防御。");

        // 统计当前拥有的士兵蚁数量
        int currentSoldiers = FindObjectsByType<Card>(FindObjectsSortMode.None)
            .Count(c => c.CardData == soldierAntCardData);

        Debug.Log($"当前士兵蚁数量: {currentSoldiers}");

        if (currentSoldiers < requiredSoldiers)
        {
            // 士兵不足，游戏结束
            GameOver($"游戏结束：入侵失败！\n你需要 {requiredSoldiers} 名士兵蚁来抵御入侵，但只有 {currentSoldiers} 名。");
        }
        else
        {
            // 防御成功
            Debug.Log("防御成功！蚁群幸存了下来。");
            // （未来可以添加士兵伤亡等逻辑）
        }
    }

    /// <summary>
    /// 游戏结束处理
    /// </summary>
    /// <param name="reason">游戏结束的原因</param>
    private void GameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.LogError("==========================================");
        Debug.LogError(reason);
        Debug.LogError("==========================================");

        // 暂停全局时间
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.PauseTimer();
        }

        // --- 核心修改点：结束游戏 ---
        Debug.LogWarning("游戏结束！正在关闭程序...");

        // 如果在Unity编辑器中运行，则停止播放模式
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 如果是编译后的游戏，则关闭程序
        Application.Quit();
#endif
    }
}