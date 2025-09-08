// 放置於: ScoutingZoneClickHandler.cs (终极调试版)
using UnityEngine;
using UnityEngine.EventSystems; // 需要引入这个命名空间

public class ScoutingZoneClickHandler : MonoBehaviour
{
    // 在Unity编辑器中，将你的ScoutingUIManager拖到这里
    public ScoutingUIManager uiManager;

    // OnMouseDown 在物体被点击时调用
    private void OnMouseDown()
    {
        // 路标 1: 检查点击事件是否被触发
        Debug.Log("--- OnMouseDown in ScoutingZoneClickHandler was called! ---");

        // 新增检查：如果鼠标指针正悬停在任何UI元素上，就不执行后续逻辑
        // 这是一个双重保险，防止在UI上方的点击穿透到游戏物体
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.LogWarning("Click was blocked because the pointer is over a UI element.");
            return;
        }

        // 路标 2: 检查 uiManager 是否被正确赋值
        if (uiManager != null)
        {
            Debug.Log("uiManager is NOT null. Attempting to call ToggleScoutingWindow...");
            // 当该物体被鼠标点击时，调用UI管理器来打开窗口
            uiManager.ToggleScoutingWindow();
        }
        else
        {
            // 如果 uiManager 是空的，这会是一条关键线索
            Debug.LogError("CRITICAL ERROR: The 'uiManager' variable in ScoutingZoneClickHandler is NULL (empty). Please drag the UI Manager object into its slot in the Inspector!");
        }
    }
}