// 文件名: ScoutWindowController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoutWindowController : MonoBehaviour
{
    [Header("核心关联")]
    [Tooltip("将你的侦察区域选择窗口Panel拖到这里")]
    public GameObject selectionWindow;

    [Tooltip("将场景中含有 ScoutingZone 脚本的那个物体拖到这里")]
    public ScoutingZone scoutingZoneTarget;

    [Header("手动设置按钮")]
    [Tooltip("在这里手动关联你的每一个按钮和它对应的侦察区域")]
    public List<ScoutButtonLink> scoutingButtons;

    // 在脚本启动时，自动为所有按钮添加点击功能
    void Start()
    {
        if (selectionWindow == null || scoutingZoneTarget == null)
        {
            Debug.LogError("请在Inspector中设置好 Selection Window 和 Scouting Zone Target！");
            return;
        }

        // 默认隐藏窗口
        selectionWindow.SetActive(false);

        // 遍历你在Inspector中设置的所有按钮
        foreach (var link in scoutingButtons)
        {
            // 确保按钮和LootTable都设置了
            if (link.button != null && link.lootTable != null)
            {
                // 为按钮添加点击事件，当点击时，调用SelectArea函数
                link.button.onClick.AddListener(() => SelectArea(link.lootTable));
            }
        }
    }

    // 当一个侦察按钮被点击时，会调用这个函数
    private void SelectArea(LootTable selectedLootTable)
    {
        Debug.Log("选择了新的侦察区域: " + selectedLootTable.name);
        scoutingZoneTarget.SetScoutingArea(selectedLootTable);
        selectionWindow.SetActive(false); // 选择后关闭窗口
    }

    // 这个公共函数可以被其他UI元素（比如打开侦察窗口的按钮）调用
    public void ToggleSelectionWindow()
    {
        if (selectionWindow != null)
        {
            selectionWindow.SetActive(!selectionWindow.activeSelf);
        }
    }
}

// 这是一个简单的数据容器，用来在Inspector里把按钮和LootTable绑在一起
[System.Serializable]
public class ScoutButtonLink
{
    public Button button;
    public LootTable lootTable;
}