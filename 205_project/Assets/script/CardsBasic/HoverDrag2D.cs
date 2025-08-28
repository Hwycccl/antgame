// HoverDrag2D.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoverDrag2D : MonoBehaviour
{
    [Header("视觉效果")]
    public float hoverScaleFactor = 0.85f;
    public float pressScaleFactor = 0.7f;
    public float lerpSpeed = 5f;
    public float hoverCooldownTime = 1f;

    [Header("渲染层级")]
    public int sortingOrderOnDrag = 100;
    public SpriteRenderer artworkRenderer; // 指向 Artwork

    private Vector3 originalScale;
    private bool isDragging = false;
    private Vector3 dragOffset;
    private bool isHovering = false;
    private bool hoverCooldown = false;
    private float hoverCooldownTimer = 0f;

    private STACK2D stackScript;
    private SpriteRenderer backgroundRenderer;
    private int originalSortingOrder;

    void Start()
    {
        originalScale = transform.localScale;
        stackScript = GetComponent<STACK2D>();
        backgroundRenderer = GetComponent<SpriteRenderer>();
        if (artworkRenderer != null)
            originalSortingOrder = artworkRenderer.sortingOrder;
        else if (backgroundRenderer != null)
            originalSortingOrder = backgroundRenderer.sortingOrder;
    }

    void Update()
    {
        if (hoverCooldown)
        {
            hoverCooldownTimer -= Time.deltaTime;
            if (hoverCooldownTimer <= 0f) hoverCooldown = false;
        }

        if (!isDragging && !hoverCooldown)
            CheckHover();
        else
            isHovering = false;

        Vector3 desiredScale = originalScale;
        if (isDragging)
            desiredScale = originalScale * pressScaleFactor;
        else if (isHovering)
            desiredScale = originalScale * hoverScaleFactor;

        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * lerpSpeed);

        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;
            transform.position = mouseWorldPos + dragOffset;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        isHovering = false;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

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
        hoverCooldown = true;
        hoverCooldownTimer = hoverCooldownTime;

        if (stackScript != null)
            stackScript.EndDrag();
        else
            ResetSortingOrder();
    }

    private void CheckHover()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        isHovering = (hit.collider != null && hit.collider.gameObject == gameObject);
    }

    public void ForceReset()
    {
        isDragging = false;
        isHovering = false;
        transform.localScale = originalScale;
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
