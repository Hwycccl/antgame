// HoverDrag2D.cs (推荐用于正交摄像机的最终版本)
using UnityEngine;

[RequireComponent(typeof(CardsBehaviour))]
public class HoverDrag2D : MonoBehaviour
{
    private CardsBehaviour cardsBehaviour;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private Vector3 offset;
    private int originalSortingOrder;

    private float distanceToCamera; // 用于存储拖拽开始时的固定摄像机距离

    [Header("拖拽時提升的渲染層級")]
    [Tooltip("拖拽時，將卡牌的 Order in Layer 提升到這個值，確保它在最上層")]
    public int sortingOrderOnDrag = 100;

    void Awake()
    {
        cardsBehaviour = GetComponent<CardsBehaviour>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        spriteRenderer = cardsBehaviour.GetArtworkRenderer();
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    void OnMouseDown()
    {
        // 1. 在拖拽开始时，计算一次卡牌平面到摄像机的距离并存储
        //    对于正交摄像机，这确保了坐标转换的稳定性
        distanceToCamera = mainCamera.WorldToScreenPoint(transform.position).z;

        // 2. 计算鼠标点击位置与卡牌中心的偏移量
        offset = transform.position - GetMouseWorldPos();

        // 3. 提升渲染层级，让卡牌显示在最上面
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrderOnDrag;
        }

        // 4. 通知 CardsBehaviour 拖拽已开始
        if (cardsBehaviour != null)
        {
            cardsBehaviour.BeginDrag();
        }
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        if (cardsBehaviour != null)
        {
            cardsBehaviour.EndDrag();
        }
    }

    // 将鼠标的屏幕坐标转换为世界坐标
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        // Z 轴的值使用拖拽开始时存储的固定距离
        mousePoint.z = distanceToCamera;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void ResetSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    public void SetNewOriginalOrder(int newOrder)
    {
        originalSortingOrder = newOrder;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = newOrder;
        }
    }
}