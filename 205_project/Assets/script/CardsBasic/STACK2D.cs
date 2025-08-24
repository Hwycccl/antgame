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
    public string[] validTags = new string[] { "ants" };

    [Header("高亮提示")]
    public GameObject borderObject;

    private bool isDragging = false;
    private GameObject nearestStackTarget = null;

    private COMBINE2D combineScript;
    private HoverDrag2D hoverDragScript;

    void Start()
    {
        if (borderObject != null)
            borderObject.SetActive(false);
        combineScript = GetComponent<COMBINE2D>();
        hoverDragScript = GetComponent<HoverDrag2D>();
    }

    void Update()
    {
        if (isDragging)
        {
            // HoverDrag2D 负责移动，这里只负责检测
            if (combineScript != null && combineScript.IsInCombination())
            {
                var partner = combineScript.GetPartner();
                if (partner != null && Vector2.Distance(transform.position, partner.transform.position) > detectRadius)
                {
                    combineScript.CancelCombination();
                }
            }
            else
            {
                CheckNearbyObjects();
            }
        }
    }

    public void StartDrag()
    {
        isDragging = true;

        if (combineScript != null && combineScript.IsInCombination())
        {
            combineScript.PauseCombination();
            var partner = combineScript.GetPartner();
            if (partner != null) partner.PauseCombination();
        }
    }

    public void EndDrag()
    {
        isDragging = false;

        if (borderObject != null)
            borderObject.SetActive(false);

        if (combineScript != null && combineScript.IsInCombination())
        {
            var partner = combineScript.GetPartner();
            if (partner != null && Vector2.Distance(transform.position, partner.transform.position) <= detectRadius)
            {
                StartCoroutine(SnapAndResume(partner.gameObject));
            }
            else
            {
                combineScript.CancelCombination();
            }
        }
        else if (nearestStackTarget != null)
        {
            StartCoroutine(SnapAndAttemptCombine(nearestStackTarget));
        }
        else
        {
            if (hoverDragScript != null) hoverDragScript.ResetSortingOrder();
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
            var targetCombine = hit.GetComponent<COMBINE2D>();
            if (targetCombine != null && targetCombine.IsInCombination()) continue;
            if (!validTags.Any(tag => hit.gameObject.CompareTag(tag))) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestStackTarget = hit.gameObject;
            }
        }

        if (borderObject != null)
        {
            borderObject.SetActive(nearestStackTarget != null);
        }
    }

    private IEnumerator SnapAndAttemptCombine(GameObject target)
    {
        var targetHoverDrag = target.GetComponent<HoverDrag2D>();

        // 1. 禁用脚本
        if (hoverDragScript) hoverDragScript.enabled = false;
        if (targetHoverDrag) targetHoverDrag.enabled = false;

        // 2. 执行吸附
        yield return SnapToTarget(target);

        // 3. 尝试组合
        var targetCombine = target.GetComponent<COMBINE2D>();
        bool combinationStarted = false;
        if (combineScript != null && targetCombine != null)
        {
            combinationStarted = combineScript.AttemptToStartCombination(targetCombine, true);
        }

        // 4. **核心修正**: 动画结束后，如果组合没有开始，则立刻恢复脚本
        if (!combinationStarted)
        {
            if (hoverDragScript) hoverDragScript.enabled = true;
            if (targetHoverDrag) targetHoverDrag.enabled = true;
        }
    }

    private IEnumerator SnapAndResume(GameObject target)
    {
        yield return SnapToTarget(target);

        if (combineScript != null)
        {
            combineScript.ResumeCombination();
            var partner = combineScript.GetPartner();
            if (partner != null) partner.ResumeCombination();
        }
    }

    private IEnumerator SnapToTarget(GameObject target)
    {
        if (hoverDragScript) hoverDragScript.ForceReset();

        var thisRenderer = GetComponent<SpriteRenderer>();
        var targetRenderer = target.GetComponent<SpriteRenderer>();
        if (thisRenderer != null && targetRenderer != null)
        {
            thisRenderer.sortingOrder = targetRenderer.sortingOrder + 1;
        }

        Vector3 targetPosition = target.transform.position + stackOffset;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            yield return null;
        }
        transform.position = targetPosition;
    }
}