using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class EncyclopediaUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject scienceSubCategoryContainer;
    public GameObject cardSubCategoryContainer;
    public TextMeshProUGUI detailTitleText;
    public TextMeshProUGUI detailContentText;

    [Header("预制体引用")]
    public Button categoryButtonPrefab;

    // --- 修改点 #1: 不再需要手动拖拽数据库 ---
    // public CardsDataBase allCardsDatabase; // <-- 删除或注释掉这一行

    private Dictionary<string, string> scienceContent = new Dictionary<string, string>();

    void Start()
    {
        scienceSubCategoryContainer.SetActive(false);
        cardSubCategoryContainer.SetActive(false);
        detailTitleText.text = "Welcome to the Encyclopedia";
        detailContentText.text = "Please click on a category to the left to see the details.";

        PopulateScienceData();
        CreateScienceUI();
        // 不再在Start()中调用CreateCardEncyclopediaUI()，因为要确保CardsDataBase已经初始化
    }

    // ... (PopulateScienceData, CreateScienceUI, OnScienceEntryClicked 这几个方法保持不变) ...
    #region 
    void PopulateScienceData()
    {
        scienceContent.Add("Characteristics",
            "Leaf-cutter ants are renowned for their unique agricultural behavior. They remain in their nests during the night and swarm out at dawn to harvest leaves. Instead of consuming the leaves directly, they use the fragments to cultivate a special fungus, which serves as their primary food source. Their eggs are milky-white and have an irregular oval shape.");
        scienceContent.Add("Division of Labor",
            "The colony has a highly organized social structure. <b>Minima ants</b>, the smallest workers, tend to the eggs, larvae, and the fungus gardens. <b>Minor ants</b> are the most numerous guards, protecting the foraging Media ants. <b>Media ants</b> are the main foragers, responsible for cutting and carrying leaf fragments. <b>Major ants (Soldiers)</b>, the largest caste, primarily defend the nest. The <b>Queen</b> is the sole reproductive member, focused on laying eggs.");
        scienceContent.Add("Diet",
            "The diet of leaf-cutter ants varies between adults and larvae. Adult ants primarily feed on the sap from the leaves they cut for energy. The meticulously cultivated fungus, however, is exclusively fed to the growing larvae.");
        scienceContent.Add("Life Cycle",
            "The life cycle of a leaf-cutter ant is a complete metamorphosis, progressing through four distinct stages: from Egg, it hatches into a Larva, then develops into a Pupa, and finally emerges as an Adult.");
        scienceContent.Add("Growth and Reproduction",
            "A new leaf-cutter ant kingdom begins with the Queen's nuptial flight. After mating mid-air, a new queen lands, sheds her wings, and finds a suitable location to establish a new nest. There, she starts cultivating her very first fungus garden, laying the foundation for the new colony.");
        scienceContent.Add("Fungus Garden System",
            "The fungus garden is the core of the leaf-cutter ant society. Worker ants bring leaf fragments to the nest, chew them into a pulp, and use it to grow their symbiotic fungus. Minima ants often \"hitchhike\" on these leaf fragments, protecting them from pests and applying antibiotics to prevent harmful mold, ensuring the health of the garden and the stability of the colony's food supply.");
    }
    void CreateScienceUI()
    {
        foreach (Transform child in scienceSubCategoryContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in scienceContent)
        {
            Button buttonInstance = Instantiate(categoryButtonPrefab, scienceSubCategoryContainer.transform);
            buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = entry.Key;
            buttonInstance.onClick.AddListener(() => OnScienceEntryClicked(entry.Key, entry.Value));
        }
    }
    void OnScienceEntryClicked(string title, string content)
    {
        detailTitleText.text = title;
        detailContentText.text = content;
    }
    #endregion


    #region 
    void CreateCardEncyclopediaUI()
    {
        foreach (Transform child in cardSubCategoryContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // --- 修改点 #2: 通过单例实例来获取所有卡牌 ---
        List<CardsBasicData> allCards = CardsDataBase.Instance.GetAllCards();

        if (allCards == null || allCards.Count == 0)
        {
            Debug.LogError("无法从CardsDataBase.Instance获取到卡牌列表，或者列表为空！");
            return;
        }

        Dictionary<CardsBasicData.CardType, List<CardsBasicData>> unlockedCardsByType = new Dictionary<CardsBasicData.CardType, List<CardsBasicData>>();

        foreach (var cardData in allCards)
        {
            if (UnlockedCardsManager.IsCardUnlocked(cardData.cardName))
            {
                if (!unlockedCardsByType.ContainsKey(cardData.cardType))
                {
                    unlockedCardsByType[cardData.cardType] = new List<CardsBasicData>();
                }
                unlockedCardsByType[cardData.cardType].Add(cardData);
            }
        }

        foreach (var cardTypePair in unlockedCardsByType)
        {
            foreach (var cardData in cardTypePair.Value)
            {
                Button buttonInstance = Instantiate(categoryButtonPrefab, cardSubCategoryContainer.transform);
                buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = cardData.cardName;
                buttonInstance.onClick.AddListener(() => OnCardEntryClicked(cardData));
            }
        }
    }

    void OnCardEntryClicked(CardsBasicData cardData)
    {
        detailTitleText.text = cardData.cardName;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<b>Description:</b> {cardData.description}");
        sb.AppendLine($"<b>Fungus Cost:</b> {cardData.fungusCost}");
        sb.AppendLine($"<b>Health:</b> {cardData.health}");
        sb.AppendLine($"<b>Attack:</b> {cardData.attack}");

        if (cardData is AntBasicData antData)
        {
            sb.AppendLine($"<b>GrowthTime:</b> {antData.growthTime}"); // 注意：你的AntBasicData里有growthTime
            sb.AppendLine($"<b>Work Efficiency:</b> {antData.workEfficiency}");
        }
        else if (cardData is AntNestData antNestData)
        {
            sb.AppendLine($"<b>Population Capacity:</b> {antNestData.populationCapacity}");
        }
        else if (cardData is BuildingBasicData buildingData)
        {
            sb.AppendLine($"<b>Max HP:</b> {buildingData.maxHp}");
        }
        else if (cardData is ResourceBasicData resourceData)
        {
            sb.AppendLine($"<b>Resource Value:</b> {resourceData.resourceValue}");
        }

        detailContentText.text = sb.ToString();
    }
    #endregion

    #region 
    public void ToggleSciencePanel()
    {
        scienceSubCategoryContainer.SetActive(!scienceSubCategoryContainer.activeSelf);
    }

    // --- 修改点 #3: 改造ToggleCardPanel方法 ---
    public void ToggleCardPanel()
    {
        // 切换显示状态
        bool isActive = !cardSubCategoryContainer.activeSelf;
        cardSubCategoryContainer.SetActive(isActive);

        // 如果面板被打开，就刷新UI
        if (isActive)
        {
            Debug.Log("卡牌百科面板已打开，正在刷新列表...");
            // 确保CardsDataBase单例已存在
            if (CardsDataBase.Instance != null)
            {
                CreateCardEncyclopediaUI();
            }
            else
            {
                Debug.LogError("CardsDataBase.Instance为空！请确保场景中有一个带有CardsDataBase脚本的激活物体。");
            }
        }
    }
    #endregion
}