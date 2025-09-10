// �ļ���: ScoutWindowController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoutWindowController : MonoBehaviour
{
    [Header("���Ĺ���")]
    [Tooltip("������������ѡ�񴰿�Panel�ϵ�����")]
    public GameObject selectionWindow;

    [Tooltip("�������к��� ScoutingZone �ű����Ǹ������ϵ�����")]
    public ScoutingZone scoutingZoneTarget;

    [Header("�ֶ����ð�ť")]
    [Tooltip("�������ֶ��������ÿһ����ť������Ӧ���������")]
    public List<ScoutButtonLink> scoutingButtons;

    // �ڽű�����ʱ���Զ�Ϊ���а�ť��ӵ������
    void Start()
    {
        if (selectionWindow == null || scoutingZoneTarget == null)
        {
            Debug.LogError("����Inspector�����ú� Selection Window �� Scouting Zone Target��");
            return;
        }

        // Ĭ�����ش���
        selectionWindow.SetActive(false);

        // ��������Inspector�����õ����а�ť
        foreach (var link in scoutingButtons)
        {
            // ȷ����ť��LootTable��������
            if (link.button != null && link.lootTable != null)
            {
                // Ϊ��ť��ӵ���¼��������ʱ������SelectArea����
                link.button.onClick.AddListener(() => SelectArea(link.lootTable));
            }
        }
    }

    // ��һ����찴ť�����ʱ��������������
    private void SelectArea(LootTable selectedLootTable)
    {
        Debug.Log("ѡ�����µ��������: " + selectedLootTable.name);
        scoutingZoneTarget.SetScoutingArea(selectedLootTable);
        selectionWindow.SetActive(false); // ѡ���رմ���
    }

    // ��������������Ա�����UIԪ�أ��������촰�ڵİ�ť������
    public void ToggleSelectionWindow()
    {
        if (selectionWindow != null)
        {
            selectionWindow.SetActive(!selectionWindow.activeSelf);
        }
    }
}

// ����һ���򵥵�����������������Inspector��Ѱ�ť��LootTable����һ��
[System.Serializable]
public class ScoutButtonLink
{
    public Button button;
    public LootTable lootTable;
}