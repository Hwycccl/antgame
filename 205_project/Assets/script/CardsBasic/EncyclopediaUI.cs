// 放置于: EncyclopediaUI.cs (最终版，已集成打开/关闭按钮联动功能)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class EncyclopediaUI : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("百科的全屏UI面板，用于整体显示和隐藏")]
    public GameObject encyclopediaPanel;
    public GameObject scienceSubCategoryContainer;
    public GameObject cardSubCategoryContainer;
    public TextMeshProUGUI detailTitleText;
    public TextMeshProUGUI detailContentText;

    [Header("按钮引用")]
    [Tooltip("主UI上用于打开百科面板的按钮")]
    public Button openButton; // <-- 新增：对“打开”按钮的引用
    [Tooltip("百科面板上用于关闭整个面板的按钮")]
    public Button closeButton;
    public Button categoryButtonPrefab;

    private Dictionary<string, string> scienceContent = new Dictionary<string, string>();

    void Start()
    {
        // 初始化时，确保面板是隐藏的，而打开按钮是显示的
        if (encyclopediaPanel != null) encyclopediaPanel.SetActive(false);
        if (openButton != null) openButton.gameObject.SetActive(true);

        scienceSubCategoryContainer.SetActive(false);
        cardSubCategoryContainer.SetActive(false);
        detailTitleText.text = "Welcome to the Encyclopedia";
        detailContentText.text = "Please click on a category to the left to see the details.";

        PopulateScienceData();
        CreateScienceUI();

        // 绑定按钮的点击事件
        if (openButton != null)
        {
            openButton.onClick.AddListener(ShowEncyclopediaPanel);
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideEncyclopediaPanel);
        }
    }

    /// <summary>
    /// 显示百科全书UI面板，并隐藏打开按钮。
    /// </summary>
    public void ShowEncyclopediaPanel()
    {
        if (encyclopediaPanel != null)
        {
            encyclopediaPanel.SetActive(true);
        }
        // --- 核心修改点 #1: 隐藏打开按钮 ---
        if (openButton != null)
        {
            openButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 隐藏百科全书UI面板，并显示打开按钮。
    /// </summary>
    public void HideEncyclopediaPanel()
    {
        if (encyclopediaPanel != null)
        {
            encyclopediaPanel.SetActive(false);
        }
        // --- 核心修改点 #2: 显示打开按钮 ---
        if (openButton != null)
        {
            openButton.gameObject.SetActive(true);
        }
    }

    // ... (后续所有其他方法保持不变) ...
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
            sb.AppendLine($"<b>GrowthTime:</b> {antData.growthTime}");
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

    public void ToggleCardPanel()
    {
        bool isActive = !cardSubCategoryContainer.activeSelf;
        cardSubCategoryContainer.SetActive(isActive);

        if (isActive)
        {
            Debug.Log("卡牌百科面板已打开，正在刷新列表...");
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