// 放置於: ScoutingUIManager.cs (最终正确版)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoutingUIManager : MonoBehaviour
{
    public static ScoutingUIManager Instance { get; private set; }

    [Header("UI 元素")]
    [Tooltip("侦察区域选择窗口的Panel")]
    public GameObject scoutingWindow;
    // 注意：OpenScoutingButton 已经被移除了
    [Tooltip("放置选择按钮的容器")]
    public Transform buttonContainer;
    [Tooltip("选择按钮的预制件")]
    public GameObject buttonPrefab;

    [Header("逻辑关联")]
    [Tooltip("场景中的ScoutingZone脚本")]
    public ScoutingZone scoutingZone;

    void Awake()
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

    void Start()
    {
        if (scoutingWindow != null)
        {
            scoutingWindow.SetActive(false); // 默认隐藏窗口
        }

        // 注意：关于 OpenScoutingButton 的监听器代码也已经被移除了
        GenerateScoutingButtons();
    }

    // 生成侦察区域选择按钮
    void GenerateScoutingButtons()
    {
        // 清理旧按钮
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        if (scoutingZone == null || scoutingZone.availableScoutAreas == null)
        {
            Debug.LogError("ScoutingZone or its available areas are not set in the ScoutingUIManager!");
            return;
        }

        // 根据ScoutingZone中的数据创建按钮
        foreach (var areaData in scoutingZone.availableScoutAreas)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);

            // 为了安全，检查预制件是否有Text和Image组件
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = areaData.areaName;

            Image buttonImage = buttonGO.GetComponent<Image>();
            if (buttonImage != null) buttonImage.sprite = areaData.areaImage;

            Button button = buttonGO.GetComponent<Button>();
            LootTable lootTable = areaData.lootTable;
            button.onClick.AddListener(() => {
                SelectAreaAndCloseWindow(lootTable);
            });
        }
    }

    // 选择区域并关闭窗口
    void SelectAreaAndCloseWindow(LootTable selectedLootTable)
    {
        if (scoutingZone != null)
        {
            scoutingZone.SetScoutingArea(selectedLootTable);
        }
        scoutingWindow.SetActive(false);
    }

    // 这个函数现在由 ScoutingZoneClickHandler 直接调用
    public void ToggleScoutingWindow()
    {
        if (scoutingWindow != null)
        {
            bool isActive = scoutingWindow.activeSelf;
            scoutingWindow.SetActive(!isActive);
            Debug.Log($"Toggling scouting window. Current state: {isActive}, New state: {!isActive}");
        }
        else
        {
            Debug.LogError("The 'scoutingWindow' is not assigned in the ScoutingUIManager!");
        }
    }
}
