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

    private COMBINE2D combineScript;
    private HoverDrag2D hoverDragScript;
    private SpriteRenderer artworkRenderer;

    void Start()
    {
        if (borderObject != null)
            borderObject.SetActive(false);

        combineScript = GetComponent<COMBINE2D>();
        hoverDragScript = GetComponent<HoverDrag2D>();
        if (hoverDragScript != null)
            artworkRenderer = hoverDragScript.artworkRenderer;
    }

    void Update()
    {
        if (isDragging)
        {
            if (combineScript != null && combineScript.IsInCombination())
            {
                var partner = combineScript.GetPartner();
                if (partner != null && Vector2.Distance(transform.position, partner.transform.position) > detectRadius)
                    combineScript.CancelCombination();
            }
            else
            {
                CheckNearbyObjects();
            }
        }
    }

    public void StartDrag() => isDragging = true;

    public void EndDrag()
    {
        isDragging = false;

        if (borderObject != null)
            borderObject.SetActive(false);

        if (nearestStackTarget != null)
        {
            StartCoroutine(SnapAndAttemptCombine(nearestStackTarget));
        }
        else if (hoverDragScript != null)
        {
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

    private IEnumerator SnapAndAttemptCombine(GameObject target)
    {
        var targetHover = target.GetComponent<HoverDrag2D>();

        if (hoverDragScript) hoverDragScript.enabled = false;
        if (targetHover) targetHover.enabled = false;

        yield return SnapToTarget(target);

        var targetCombine = target.GetComponent<COMBINE2D>();
        bool combinationStarted = false;
        if (combineScript != null && targetCombine != null)
            combinationStarted = combineScript.AttemptToStartCombination(targetCombine, true);

        if (!combinationStarted)
        {
            if (hoverDragScript) hoverDragScript.enabled = true;
            if (targetHover) targetHover.enabled = true;
        }
    }

    private IEnumerator SnapToTarget(GameObject target)
    {
        if (hoverDragScript) hoverDragScript.ForceReset();

        var targetArtwork = target.GetComponent<HoverDrag2D>()?.artworkRenderer;
        if (artworkRenderer != null && targetArtwork != null)
            artworkRenderer.sortingOrder = targetArtwork.sortingOrder + 1;

        Vector3 targetPosition = target.transform.position + stackOffset;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
