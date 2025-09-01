// HoverDrag2D.cs (最終簡化版)
using UnityEngine;

[RequireComponent(typeof(CardsBehaviour))]
public class HoverDrag2D : MonoBehaviour
{
    private CardsBehaviour cardsBehaviour;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private Vector3 offset;
    private int originalSortingOrder;

    [Header("拖拽時提升的渲染層級")]
    [Tooltip("拖拽時，將卡牌的 Order in Layer 提升到這個值，確保它在最上層")]
    public int sortingOrderOnDrag = 100;

    void Awake()
    {
        cardsBehaviour = GetComponent<CardsBehaviour>();
        mainCamera = Camera.main; // 獲取主攝影機的引用
    }

    void Start()
    {
        // 從 CardsBehaviour 獲取 SpriteRenderer
        spriteRenderer = cardsBehaviour.GetArtworkRenderer();
        if (spriteRenderer != null)
        {
            // 儲存原始的渲染順序
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    void OnMouseDown()
    {
        // --- 拖拽開始 ---
        // 1. 計算滑鼠點擊位置與卡牌中心的偏移量
        offset = transform.position - GetMouseWorldPos();

        // 2. 提升渲染層級，讓卡牌顯示在最上面
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrderOnDrag;
        }

        // 3. 通知 CardsBehaviour 拖拽已開始
        if (cardsBehaviour != null)
        {
            cardsBehaviour.BeginDrag();
        }
    }

    void OnMouseDrag()
    {
        // --- 拖拽過程中 ---
        // 持續更新卡牌的位置，使其跟隨滑鼠（並保持偏移量）
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        // --- 拖拽結束 ---
        // 通知 CardsBehaviour 拖拽已結束，讓它處理堆疊、合成或歸位的邏輯
        if (cardsBehaviour != null)
        {
            cardsBehaviour.EndDrag();
        }
    }

    // 將滑鼠的螢幕座標轉換為世界座標
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        // Z 軸的值需要設定為攝影機到物體的距離
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // 公共方法，用於在邏輯處理後（如歸位）恢復原始的渲染順序
    public void ResetSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    // 公共方法，允許外部腳本（如STACK2D）在堆疊後更新此卡片的“原始”渲染順序
    public void SetNewOriginalOrder(int newOrder)
    {
        originalSortingOrder = newOrder;
        // 同時也更新當前的渲染順序，因為它已經堆疊好了
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = newOrder;
        }
    }
}
