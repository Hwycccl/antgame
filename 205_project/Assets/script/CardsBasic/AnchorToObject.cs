
using UnityEngine;

/// <summary>
/// 将一个非UI的世界物体“锚定”到屏幕的固定位置。
/// </summary>
public class AnchorToScreen : MonoBehaviour
{
    [Header("摄像机")]
    [Tooltip("指定用于计算位置的主摄像机")]
    [SerializeField] private Camera mainCamera;

    [Header("锚点设置")]
    [Tooltip("要锚定的标准化屏幕坐标。(0,0)是左下角, (1,1)是右上角")]
    [SerializeField] private Vector2 screenAnchorPoint = new Vector2(1, 1);

    [Tooltip("物体与摄像机之间的距离（Z轴）")]
    [SerializeField] private float distanceFromCamera = 10f;

    void Awake()
    {
        // 如果没有手动指定摄像机，则尝试自动查找场景中的主摄像机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // 使用LateUpdate可以确保在摄像机完成所有移动和旋转之后再更新物体位置
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            Debug.LogError("没有找到或指定摄像机！");
            return;
        }

        // 1. 将标准化的锚点坐标 (0-1范围) 转换为屏幕像素坐标
        //    (虽然ViewportToWorldPoint可以直接用0-1坐标，但这里为了清晰展示过程)
        //    Vector3 screenPoint = new Vector3(Screen.width * screenAnchorPoint.x, Screen.height * screenAnchorPoint.y, distanceFromCamera);

        // 2. 更直接的方式：使用 ViewportToWorldPoint 函数
        //    这个函数直接使用0-1范围的视口坐标，更适合跨分辨率的场景。
        //    Vector3 viewportPoint = new Vector3(screenAnchorPoint.x, screenAnchorPoint.y, distanceFromCamera);

        // 3. 计算出目标世界坐标
        //    transform.position = mainCamera.ScreenToWorldPoint(screenPoint); // 对应上面的方法1
        transform.position = mainCamera.ViewportToWorldPoint(new Vector3(screenAnchorPoint.x, screenAnchorPoint.y, distanceFromCamera)); // 对应上面的方法2
    }
}