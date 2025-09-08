// 放置于: ElevationController.cs
// 并将其附加到你所有的卡牌预制件上
using UnityEngine;
using UnityEngine.UI;

public class ElevationController : MonoBehaviour
{
    private Canvas temporaryCanvas;
    private GraphicRaycaster temporaryRaycaster;
    private bool isElevated = false;

    // 一个公共方法，用于控制提升状态
    public void SetElevated(bool elevated)
    {
        if (isElevated == elevated) return; // 如果状态没变，就什么都不做

        isElevated = elevated;

        if (elevated)
        {
            // --- 提升层级 ---
            if (temporaryCanvas == null)
            {
                temporaryCanvas = gameObject.AddComponent<Canvas>();
                temporaryCanvas.overrideSorting = true;
                temporaryCanvas.sortingOrder = 30000; // 确保在最顶层

                temporaryRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        else
        {
            // --- 恢复层级 ---
            if (temporaryRaycaster != null) Destroy(temporaryRaycaster);
            if (temporaryCanvas != null) Destroy(temporaryCanvas);
        }
    }
}
