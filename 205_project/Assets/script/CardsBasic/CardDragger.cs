// 放置於: CardDragger.cs (修正牌堆拖拽層級版)
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;

    // 我們需要一個引用來記住被提升層級的根卡牌
    private CardStacker rootStackerOfDraggedStack;
    private int originalRootSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        card.Stacker.OnBeginDrag();

        zCoordinate = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();

        // --- 核心修改點 開始 ---

        // 1. 找到被拖動牌堆的根卡牌 (Root)
        rootStackerOfDraggedStack = card.Stacker.GetRoot();

        // 2. 只獲取並修改根卡牌的 SpriteRenderer
        var rootRenderer = rootStackerOfDraggedStack.GetComponent<Card>().GetArtworkRenderer();
        if (rootRenderer != null)
        {
            // 3. 記錄並提升根卡牌的渲染層級
            originalRootSortingOrder = rootRenderer.sortingOrder;
            rootRenderer.sortingOrder = dragSortingOrder;

            // 4. 立刻更新整個牌堆的視覺效果
            // 這會讓所有子卡牌的層級都根據新的根卡牌層級進行刷新
            rootStackerOfDraggedStack.UpdateStackVisuals();
        }

        // --- 核心修改點 結束 ---
    }

    void OnMouseDrag()
    {
        // 當拖動時，我們移動的是整個根卡牌的 Transform
        // 由於子卡牌都是它的子物件，所以會跟著一起移動
        rootStackerOfDraggedStack.transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        // --- 還原渲染層級的修改 ---
        if (rootStackerOfDraggedStack != null)
        {
            var rootRenderer = rootStackerOfDraggedStack.GetComponent<Card>().GetArtworkRenderer();
            if (rootRenderer != null)
            {
                // 1. 將根卡牌的層級還原
                rootRenderer.sortingOrder = originalRootSortingOrder;

                // 2. 再次更新整個牌堆的視覺，讓所有子卡牌的層級也還原
                rootStackerOfDraggedStack.UpdateStackVisuals();
            }
        }

        // --- 後續堆疊邏輯不變 ---
        card.Stacker.OnEndDrag();

        // 清理引用
        rootStackerOfDraggedStack = null;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // 這個函數現在由 UpdateStackVisuals 自動管理，但保留以防萬一
    public void SetOriginalSortingOrder(int newOrder)
    {
        // originalRootSortingOrder = newOrder;
    }
}