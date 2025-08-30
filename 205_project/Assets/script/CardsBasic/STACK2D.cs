//STACK2D.cs
// STACK2D.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class STACK2D : MonoBehaviour
{
    [Header("Detect Settings")]
    public float detectRadius = 1.5f;
    public LayerMask stackableLayer;

    [Header("Stack Settings")]
    public Vector3 stackOffset = new Vector3(0, -0.2f, 0);
    public float snapSpeed = 10f;

    [Header("Valid Tags")]
    public string[] validTags = new string[] { "Unit" };

    [Header("Highlight Border")]
    public GameObject borderObject;

    [HideInInspector] public STACK2D stackAbove;
    [HideInInspector] public STACK2D stackBelow;

    private bool isDragging = false;
    private GameObject nearestStackTarget = null;

    [HideInInspector] public HoverDrag2D hoverDragScript;
    [HideInInspector] public SpriteRenderer artworkRenderer;

    private bool isStacked = false;
    private List<STACK2D> dragStack = new List<STACK2D>();

    public bool IsStacked() => isStacked;

    void Start()
    {
        if (borderObject != null)
            borderObject.SetActive(false);

        hoverDragScript = GetComponent<HoverDrag2D>();
        if (hoverDragScript != null)
        {
            artworkRenderer = hoverDragScript.artworkRenderer;
            if (artworkRenderer == null)
            {
                Transform artTransform = transform.Find("artwork");
                if (artTransform != null)
                    artworkRenderer = artTransform.GetComponent<SpriteRenderer>();
            }
        }
    }

    void Update()
    {
        if (isDragging)
            CheckNearbyObjects();
    }

    // === 主要修改点 1: StartDrag 逻辑恢复为“分离”模式 ===
    public void StartDrag()
    {
        isDragging = true;
        isStacked = false;

        // 1. 构建拖动链：从当前被点击的卡牌 + 它上方所有卡牌
        dragStack.Clear();
        STACK2D current = this;
        int safety = 0;
        int dragIndex = 0;
        while (current != null && safety < 100)
        {
            dragStack.Add(current);
            // 2. 为拖动链中的每一张牌设置新的渲染层级
            if (current.hoverDragScript != null && current.hoverDragScript.artworkRenderer != null)
            {
                current.hoverDragScript.artworkRenderer.sortingOrder = current.hoverDragScript.sortingOrderOnDrag + dragIndex;
            }
            current = current.stackAbove;
            dragIndex++;
            safety++;
        }

        // 3. 与下方的牌断开连接
        if (stackBelow != null)
        {
            stackBelow.stackAbove = null;
            stackBelow = null;
        }
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
            // === 主要修改点 2: 如果没有堆叠，则重置被分离出来的整个堆叠的渲染层级 ===
            foreach (var card in dragStack)
            {
                if (card.hoverDragScript != null)
                    card.hoverDragScript.ResetSortingOrder();
            }
        }

        dragStack.Clear();
        nearestStackTarget = null;
    }

    void CheckNearbyObjects()
    {
        nearestStackTarget = null;
        float minDist = float.MaxValue;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, stackableLayer);
        foreach (var hit in hits)
        {
            // 防止堆叠链中的牌互相检测
            if (dragStack.Exists(card => card.gameObject == hit.gameObject)) continue;

            if (!validTags.Any(tag => hit.gameObject.CompareTag(tag))) continue;

            GameObject topCard = GetTopCardSafe(hit.gameObject);
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

    GameObject GetTopCardSafe(GameObject card)
    {
        STACK2D stack = card.GetComponent<STACK2D>();
        if (stack == null) return card;

        STACK2D current = stack;
        int safetyCounter = 0;
        while (current.stackAbove != null)
        {
            current = current.stackAbove;
            safetyCounter++;
            if (safetyCounter > 100)
            {
                Debug.LogWarning("Stack chain too long, breaking at " + current.name);
                break;
            }
        }
        return current.gameObject;
    }

    private IEnumerator SnapAndStack(GameObject target)
    {
        STACK2D targetStack = target.GetComponent<STACK2D>();
        if (targetStack != null && IsInChain(targetStack, dragStack[0]))
        {
            Debug.LogWarning("Invalid stack: would create a cycle!");
            EndDrag();
            yield break;
        }

        int baseOrder = 0;
        if (targetStack != null)
        {
            var targetHoverDrag = targetStack.GetComponent<HoverDrag2D>();
            if (targetHoverDrag != null && targetHoverDrag.artworkRenderer != null)
                baseOrder = targetHoverDrag.artworkRenderer.sortingOrder + 1;
        }

        for (int i = 0; i < dragStack.Count; i++)
        {
            if (dragStack[i].artworkRenderer != null)
                dragStack[i].artworkRenderer.sortingOrder = baseOrder + i;

            if (dragStack[i].hoverDragScript != null)
                dragStack[i].hoverDragScript.StoreNewOriginalOrder();
        }

        if (targetStack != null)
        {
            targetStack.stackAbove = dragStack[0];
            dragStack[0].stackBelow = targetStack;
        }

        for (int i = 0; i < dragStack.Count; i++)
        {
            Vector3 targetPos = (i == 0 ? target.transform.position : dragStack[i - 1].transform.position) + stackOffset;
            dragStack[i].transform.position = targetPos;
            dragStack[i].isStacked = true;
        }

        yield return null;
    }

    private bool IsInChain(STACK2D a, STACK2D b)
    {
        STACK2D current = a;
        int safetyCounter = 0;
        while (current != null)
        {
            if (current == b) return true;
            current = current.stackAbove;
            safetyCounter++;
            if (safetyCounter > 100) break;
        }
        return false;
    }

    // === 主要修改点 3: MoveDragStack 逻辑适配 ===
    // (这个方法不需要修改，因为它总是基于被点击的牌来移动整个dragStack)
    public void MoveDragStack(Vector3 newPos)
    {
        if (dragStack.Count == 0) return;
        // newPos 是被点击牌的目标位置, this.transform.position 是被点击牌的当前位置
        Vector3 delta = newPos - this.transform.position;
        foreach (var card in dragStack)
            card.transform.position += delta;
    }
}
