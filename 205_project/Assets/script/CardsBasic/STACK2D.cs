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
    public Vector3 stackOffset = new Vector3(0, -0.2f, 0);
    public float snapSpeed = 10f;

    [Header("标签验证")]
    public string[] validTags = new string[] { "Unit" };

    [Header("高亮提示")]
    public GameObject borderObject;

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
            artworkRenderer = hoverDragScript.artworkRenderer;
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
            // 没有堆叠目标才重置排序
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

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestStackTarget = hit.gameObject;
            }
        }

        if (borderObject != null)
            borderObject.SetActive(nearestStackTarget != null);
    }

    private IEnumerator SnapAndStack(GameObject target)
    {
        if (hoverDragScript) hoverDragScript.enabled = false;

        // 排序到目标上方
        var targetHover = target.GetComponent<HoverDrag2D>();
        var targetArtwork = targetHover != null ? targetHover.artworkRenderer : null;
        if (artworkRenderer != null && targetArtwork != null)
            artworkRenderer.sortingOrder = targetArtwork.sortingOrder + 1;

        // 吸附位置
        Vector3 targetPosition = target.transform.position + stackOffset;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isStacked = true;

        // 释放拖拽状态
        if (hoverDragScript) hoverDragScript.enabled = true;
    }
}
