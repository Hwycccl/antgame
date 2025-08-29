// HoverDrag2D.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoverDrag2D : MonoBehaviour
{
    [Header("Render Settings")]
    public int sortingOrderOnDrag = 100;
    public SpriteRenderer artworkRenderer; // auto fetch if null

    private Vector3 dragOffset;
    private bool isDragging = false;

    private STACK2D stackScript;
    private SpriteRenderer backgroundRenderer;
    private int originalSortingOrder;

    void Start()
    {
        stackScript = GetComponent<STACK2D>();
        backgroundRenderer = GetComponent<SpriteRenderer>();

        if (artworkRenderer == null)
        {
            Transform artTransform = transform.Find("artwork");
            if (artTransform != null)
                artworkRenderer = artTransform.GetComponent<SpriteRenderer>();
        }

        if (artworkRenderer != null)
            originalSortingOrder = artworkRenderer.sortingOrder;
        else if (backgroundRenderer != null)
            originalSortingOrder = backgroundRenderer.sortingOrder;
    }

    void Update()
    {
        if (isDragging && stackScript != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;
            Vector3 targetPos = mouseWorldPos + dragOffset;
            stackScript.MoveDragStack(targetPos);
        }
    }

    void OnMouseDown()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

        isDragging = true;

        if (artworkRenderer != null)
            artworkRenderer.sortingOrder = sortingOrderOnDrag;
        else if (backgroundRenderer != null)
            backgroundRenderer.sortingOrder = sortingOrderOnDrag;

        if (stackScript != null)
            stackScript.StartDrag();
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        if (stackScript != null)
            stackScript.EndDrag();
        else
            ResetSortingOrder();
    }

    public void ForceReset()
    {
        isDragging = false;
        transform.localScale = Vector3.one;
        ResetSortingOrder();
    }

    public void ResetSortingOrder()
    {
        if (artworkRenderer != null)
            artworkRenderer.sortingOrder = originalSortingOrder;
        else if (backgroundRenderer != null)
            backgroundRenderer.sortingOrder = originalSortingOrder;
    }
}

