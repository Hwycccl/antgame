
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    // 我们把旧的变量换成了新的、干净的ScoutWindowController
    public ScoutWindowController scoutWindowController;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 确保在Unity编辑器里设置好了这个变量
        if (scoutWindowController != null)
        {
            Debug.Log("侦察区域被点击，正在开关窗口。");
            // 调用我们新脚本里的方法
            scoutWindowController.ToggleSelectionWindow();
        }
        else
        {
            Debug.LogError("请在 " + gameObject.name + " 物体的ClickHandler组件上设置ScoutWindowController！");
        }
    }
}