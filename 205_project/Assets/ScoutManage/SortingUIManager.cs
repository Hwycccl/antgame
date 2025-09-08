// �����: ScoutingUIManager.cs (������ȷ��)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoutingUIManager : MonoBehaviour
{
    public static ScoutingUIManager Instance { get; private set; }

    [Header("UI Ԫ��")]
    [Tooltip("�������ѡ�񴰿ڵ�Panel")]
    public GameObject scoutingWindow;
    // ע�⣺OpenScoutingButton �Ѿ����Ƴ���
    [Tooltip("����ѡ��ť������")]
    public Transform buttonContainer;
    [Tooltip("ѡ��ť��Ԥ�Ƽ�")]
    public GameObject buttonPrefab;

    [Header("�߼�����")]
    [Tooltip("�����е�ScoutingZone�ű�")]
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
            scoutingWindow.SetActive(false); // Ĭ�����ش���
        }

        // ע�⣺���� OpenScoutingButton �ļ���������Ҳ�Ѿ����Ƴ���
        GenerateScoutingButtons();
    }

    // �����������ѡ��ť
    void GenerateScoutingButtons()
    {
        // ����ɰ�ť
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        if (scoutingZone == null || scoutingZone.availableScoutAreas == null)
        {
            Debug.LogError("ScoutingZone or its available areas are not set in the ScoutingUIManager!");
            return;
        }

        // ����ScoutingZone�е����ݴ�����ť
        foreach (var areaData in scoutingZone.availableScoutAreas)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);

            // Ϊ�˰�ȫ�����Ԥ�Ƽ��Ƿ���Text��Image���
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

    // ѡ�����򲢹رմ���
    void SelectAreaAndCloseWindow(LootTable selectedLootTable)
    {
        if (scoutingZone != null)
        {
            scoutingZone.SetScoutingArea(selectedLootTable);
        }
        scoutingWindow.SetActive(false);
    }

    // ������������� ScoutingZoneClickHandler ֱ�ӵ���
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
