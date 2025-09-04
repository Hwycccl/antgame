// 文件路径: Assets/script/ResourceManager.cs (已更新)
using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("UI 组件 (从场景 Hierarchy 拖入)")]
    public TextMeshProUGUI leafText;
    public TextMeshProUGUI fungusText;

    [Header("资源卡牌数据 (从项目文件夹拖入)")]
    [Tooltip("你的'叶子'资源卡牌 ScriptableObject (Leaf.asset)")]
    public ResourceBasicData leafData; // 整片叶子
    [Tooltip("你的'碎叶'资源卡牌 ScriptableObject (LeafFragment.asset)")]
    public ResourceBasicData leafFragmentData; // 碎叶
    [Tooltip("你的'真菌'资源卡牌 ScriptableObject (Fungus.asset)")]
    public ResourceBasicData fungusData;

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

    private void Update()
    {
        UpdateResourceCounts();
    }

    /// <summary>
    /// 核心函数：计算所有资源的当前总量
    /// </summary>
    public void UpdateResourceCounts()
    {
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

        // 1. 单独计算碎叶的总价值
        int leafFragmentValue = allCards
            .Where(c => c.CardData == leafFragmentData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        // 2. 单独计算完整叶子的总价值
        int leafValue = allCards
            .Where(c => c.CardData == leafData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        // 3. 按照 1叶子=2碎叶 的规则合并计算，并显示
        int totalCombinedLeafValue = leafFragmentValue + (leafValue * 2);


        // 4. 计算其他资源 (逻辑不变)
        int totalFungusValue = allCards
            .Where(c => c.CardData == fungusData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        // 将最终计算结果更新到UI上
        UpdateResourceUI(totalCombinedLeafValue, totalFungusValue);
    }

    /// <summary>
    /// 将资源数据显示在对应的UI文本上
    /// </summary>
    private void UpdateResourceUI(int leaf, int fungus)
    {
        // 这里显示的 leaf 已经是合并后的总价值了
        if (leafText != null) leafText.text = leaf.ToString();
        if (fungusText != null) fungusText.text = fungus.ToString();
    }
}