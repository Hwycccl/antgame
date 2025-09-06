using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GlobalInputHandler : MonoBehaviour
{
    void Update()
    {
        // 检测鼠标左键是否被按下
        if (Input.GetMouseButtonDown(0))
        {
            // 如果DescriptionManager不存在，或描述框本来就没显示，就什么都不做
            if (DescriptionManager.Instance == null || !DescriptionManager.Instance.descriptionBox.activeSelf)
            {
                return;
            }

            // 创建一个指针事件数据，用来进行射线检测
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            // 创建一个列表来存储所有射线检测到的结果
            List<RaycastResult> results = new List<RaycastResult>();

            // EventSystem会同时检测UI元素和带有Physics Raycaster的2D/3D物体
            EventSystem.current.RaycastAll(eventData, results);

            bool clickedOnCard = false;
            // 遍历所有检测到的物体
            foreach (RaycastResult result in results)
            {
                // 检查被点击的物体上是否有CardDragger脚本
                // 如果有，就说明我们点到了一张卡牌
                if (result.gameObject.GetComponent<CardDragger>() != null)
                {
                    clickedOnCard = true;
                    break; // 既然已经确认点到卡牌了，就没必要继续检查了
                }
            }

            // 如果遍历完所有被点击的物体，都没有发现任何一个是卡牌
            if (!clickedOnCard)
            {
                // 那么就隐藏描述框
                DescriptionManager.Instance.HideDescription();
            }
        }
    }
}
