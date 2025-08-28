// STACK2D.cs
using UnityEngine;
using System.Collections;
using System.Linq;

public class STACK2D : MonoBehaviour
{
    [Header("检测参数")]
    public float detectRadius = 1.5f;
    public LayerMask stackableLayer;

    [Header("堆叠与动画")]
    public Vector3 stackOffset = new Vector3(0, -1f, 0);
    public float snapSpeed = 10f;

    [Header("标签验证")]
    public string[] validTags = new string[] { "Unit" };

    [Header("高亮提示")]
    public GameObject borderObject;

    [HideInInspector] public STACK2D stackAbove; // 堆顶引用

    private bool isDragging = false;
    private GameObject nearestStackTarget = null;
    private HoverDrag2D hoverDragScript;
    private SpriteRenderer artworkRenderer;
    private bool isStacked = false;

    public bool IsStacked() => isStacked;

    void Start()
    {
        if (borderObject != null) borderObject.SetActive(false);

        hoverDragScript = GetComponent<HoverDrag2D>();

        if (hoverDragScript != null)
        {
            artworkRenderer = hoverDragScript.artworkRenderer;
            if (artworkRenderer == null)
            {
                // 自动获取名为 "artwork" 的子物体 SpriteRenderer
                Transform artTransform = transform.Find("artwork");
                if (artTransform != null)
                    artworkRenderer = artTransform.GetComponent<SpriteRenderer>();
            }
        }
    }

    void Update()
    {
        if (isDragging)
        {
            CheckNearbyObjects();
        }
    }

    public void StartDrag()
    {
        isDragging = true;
        isStacked = false;
        stackAbove = null;
    }

    public void EndDrag()
    {
        isDragging = false;

        if (borderObject != null)
            borderObject.SetActive(false);

        if (nearestStackTarget != null)
        {
            StartCoroutine(SnapAndStack(nearestStackTarget));
        }
        else
        {
            if (hoverDragScript != null)
                hoverDragScript.ResetSortingOrder();
        }

        nearestStackTarget = null;
    }

    void CheckNearbyObjects()
    {
        nearestStackTarget = null;
        float minDist = float.MaxValue;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, stackableLayer);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;
            if (!validTags.Any(tag => hit.gameObject.CompareTag(tag))) continue;

            GameObject topCard = GetTopCard(hit.gameObject);

            float dist = Vector2.Distance(transform.position, topCard.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestStackTarget = topCard;
            }
        }

        if (borderObject != null)
            borderObject.SetActive(nearestStackTarget != null);
    }

    GameObject GetTopCard(GameObject card)
    {
        STACK2D stack = card.GetComponent<STACK2D>();
        if (stack != null && stack.stackAbove != null)
            return GetTopCard(stack.stackAbove.gameObject);
        return card;
    }

    private IEnumerator SnapAndStack(GameObject target)
    {
        if (hoverDragScript) hoverDragScript.enabled = false;

        STACK2D targetStack = target.GetComponent<STACK2D>();
        if (artworkRenderer != null && targetStack != null && targetStack.artworkRenderer != null)
            artworkRenderer.sortingOrder = targetStack.artworkRenderer.sortingOrder + 1;

        if (targetStack != null)
            targetStack.stackAbove = this;

        Vector3 targetPosition = target.transform.position + stackOffset;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isStacked = true;

        if (hoverDragScript) hoverDragScript.enabled = true;
    }
}

