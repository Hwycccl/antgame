// 放置於: CardDragger.cs (最終診斷版)
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;
    private SpriteRenderer artworkRenderer;

    private int originalSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // OnMouseDown 已經確認是正常的，所以移除這裡的日誌
        if (mainCamera == null) { Debug.LogError("Main Camera is not found!"); return; }
        zCoordinate = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
        artworkRenderer = card.GetArtworkRenderer();
        if (artworkRenderer != null)
        {
            originalSortingOrder = artworkRenderer.sortingOrder;
            artworkRenderer.sortingOrder = dragSortingOrder;
        }
        card.Stacker.OnBeginDrag();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        Debug.Log($"[{gameObject.name}] OnMouseUp: Step 1 - Mouse button released.");
        if (artworkRenderer != null)
        {
            artworkRenderer.sortingOrder = originalSortingOrder;
        }

        Debug.Log($"[{gameObject.name}] OnMouseUp: Step 2 - Now calling OnEndDrag(). If the game freezes, the problem is inside CardStacker's logic.");

        // 呼叫堆疊邏輯，卡死預計會發生在這裡
        card.Stacker.OnEndDrag();

        // 如果你能看到下面這條日誌，代表沒有卡死
        Debug.Log($"[{gameObject.name}] OnMouseUp: Step 3 - OnEndDrag() FINISHED SUCCESSFULLY. No freeze occurred.");
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void SetOriginalSortingOrder(int newOrder)
    {
        originalSortingOrder = newOrder;
    }
}