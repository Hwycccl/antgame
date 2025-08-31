// HoverDrag2D.cs (Minimal Fix Version)
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class HoverDrag2D : MonoBehaviour
{
    [Header("Render Settings")]
    public int sortingOrderOnDrag = 100;
    public SpriteRenderer artworkRenderer;

    private Vector3 dragOffset;
    private bool isDragging = false;

    private STACK2D stackScript;
    private int originalSortingOrder;

    private static HoverDrag2D currentlyDragged;

    void Start()
    {
        stackScript = GetComponent<STACK2D>();

        if (artworkRenderer == null)
        {
            Transform artTransform = transform.Find("artwork");
            if (artTransform != null)
                artworkRenderer = artTransform.GetComponent<SpriteRenderer>();
        }

        if (artworkRenderer != null)
            originalSortingOrder = artworkRenderer.sortingOrder;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentlyDragged != null) return;

            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hits.Length > 0)
            {
                var topHit = hits
                    .Select(h => h.collider.GetComponent<HoverDrag2D>())
                    .Where(script => script != null && script.artworkRenderer != null)
                    .OrderByDescending(script => script.artworkRenderer.sortingOrder)
                    .FirstOrDefault();

                if (topHit != null)
                {
                    currentlyDragged = topHit;
                    currentlyDragged.StartDragAction();
                }
            }
        }

        if (isDragging && currentlyDragged == this)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;
            Vector3 targetPos = mouseWorldPos + dragOffset;

            if (stackScript != null)
                stackScript.MoveDragStack(targetPos);
            else
                transform.position = targetPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentlyDragged != null)
            {
                currentlyDragged.EndDragAction();
                currentlyDragged = null;
            }
        }
    }

    private void StartDragAction()
    {
        isDragging = true;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

        if (stackScript != null)
            stackScript.StartDrag();
        else if (artworkRenderer != null)
            artworkRenderer.sortingOrder = sortingOrderOnDrag;
    }

    private void EndDragAction()
    {
        isDragging = false;

        if (stackScript != null)
        {
            stackScript.EndDrag();
        }
        else
        {
            ResetSortingOrder();
        }
    }

    // === 核心修正点 ===
    // 修正了赋值方向的BUG。这是解决您问题的关键。
    public void ResetSortingOrder()
    {
        if (artworkRenderer != null)
            artworkRenderer.sortingOrder = originalSortingOrder;
    }

    public void StoreNewOriginalOrder()
    {
        if (artworkRenderer != null)
            originalSortingOrder = artworkRenderer.sortingOrder;
    }
}
