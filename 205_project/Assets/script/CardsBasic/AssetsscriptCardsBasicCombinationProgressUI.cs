// 文件路径: Assets/script/PopulationManager.cs
using UnityEngine;
using TMPro; // 需要引入 TextMeshPro 的命名空间
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    [Header("UI组件")]
    [Tooltip("用于显示人口状态的TextMeshPro文本")]
    public TextMeshProUGUI populationText;

    // --- 公共属性 ---
    public int CurrentPopulation { get; private set; }
    public int MaxPopulation { get; private set; }

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
        // 游戏开始时，立即更新一次人口数据
        UpdatePopulationStats();
    }

    // 定期更新，以防有蚂蚁被其他方式销毁
    private void Update()
    {
        // 为了性能考虑，可以不用每帧都更新，可以改为一个计时器
        // 但对于这个项目，每帧查找一次问题不大
        UpdatePopulationStats();
    }

    /// <summary>
    /// 核心函数：计算当前人口和最大人口，并更新UI
    /// </summary>
    public void UpdatePopulationStats()
    {
        // 1. 查找场景中所有的卡牌
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

        // 2. 计算当前人口 (所有类型为Ant的卡牌)
        // 这里我们假设幼虫(Larva)的cardType也设置为了Ant
        CurrentPopulation = allCards.Count(c => c.CardData != null && c.CardData.cardType == CardsBasicData.CardType.Ant);

        // 3. 计算最大人口 (所有蚁穴提供的人口总和)
        MaxPopulation = 0;
        var antNests = allCards.Where(c => c.CardData is AntNestData);
        foreach (var nestCard in antNests)
        {
            AntNestData nestData = nestCard.CardData as AntNestData;
            if (nestData != null)
            {
                MaxPopulation += nestData.populationCapacity;
            }
        }

        // 如果一个蚁穴都没有，可以给一个默认值，或者就为0
        if (MaxPopulation == 0)
        {
            // 例如，游戏开始时默认给2个位置
            // MaxPopulation = 2; 
        }

        // 4. 更新UI显示
        UpdatePopulationUI();
    }

    /// <summary>
    /// 检查蚁群是否已满
    /// </summary>
    public bool IsPopulationFull()
    {
        return CurrentPopulation >= MaxPopulation;
    }

    /// <summary>
    /// 更新UI文本
    /// </summary>
    private void UpdatePopulationUI()
    {
        if (populationText != null)
        {
            populationText.text = $"AntNumber: {CurrentPopulation} / {MaxPopulation}";

            // 如果人口满了，可以改变颜色以示提醒
            if (IsPopulationFull())
            {
                populationText.color = Color.red;
            }
            else
            {
                populationText.color = Color.white;
            }
        }
    }
}