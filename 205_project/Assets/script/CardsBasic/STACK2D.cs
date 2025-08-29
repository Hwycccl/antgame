//STACK2D.cs
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

    private HoverDrag2D hoverDragScript;
    [HideInInspector] public SpriteRenderer artworkRenderer;

    private bool isStacked = false; // 当前拖动的堆叠链
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

    public void StartDrag()
    {
        isDragging = true;
        isStacked = false;

        // 构建拖动链：当前卡 + 上方所有卡
        dragStack.Clear();
        STACK2D current = this;
        int safety = 0;
        while (current != null && safety < 50)
        {
            dragStack.Add(current);
            current = current.stackAbove;
            safety++;
        }

        // 与下方断开
        if (stackBelow != null)
        {
            stackBelow.stackAbove = null;
            stackBelow = null;
        }

        // 上方的卡也断开
        foreach (var card in dragStack)
        {
            if (card.stackAbove != null && !dragStack.Contains(card.stackAbove))
                card.stackAbove.stackBelow = null;
            card.stackAbove = null;
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
            if (hoverDragScript != null)
                hoverDragScript.ResetSortingOrder();
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
            if (hit.gameObject == this.gameObject) continue;
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
            if (safetyCounter > 50)
            {
                Debug.LogWarning("Stack chain too long, breaking at " + current.name);
                break;
            }
        }
        return current.gameObject;
    }

    private IEnumerator SnapAndStack(GameObject target)
    {
        if (hoverDragScript) hoverDragScript.enabled = false;

        STACK2D targetStack = target.GetComponent<STACK2D>();
        if (targetStack != null && IsInChain(targetStack, this))
        {
            Debug.LogWarning("Invalid stack: would create a cycle!");
            if (hoverDragScript) hoverDragScript.enabled = true;
            yield break;
        }

        // 调整拖动链渲染顺序
        int baseOrder = targetStack != null && targetStack.artworkRenderer != null ? targetStack.artworkRenderer.sortingOrder + 1 : 0;
        for (int i = 0; i < dragStack.Count; i++)
        {
            if (dragStack[i].artworkRenderer != null)
                dragStack[i].artworkRenderer.sortingOrder = baseOrder + i;
        }

        // 连接堆叠
        if (targetStack != null)
        {
            targetStack.stackAbove = dragStack[0];
            dragStack[0].stackBelow = targetStack;
        }

        // 拖动链吸附
        for (int i = 0; i < dragStack.Count; i++)
        {
            Vector3 targetPos = (i == 0 ? target.transform.position : dragStack[i - 1].transform.position) + stackOffset;
            dragStack[i].transform.position = targetPos;
            dragStack[i].isStacked = true;
        }

        if (hoverDragScript) hoverDragScript.enabled = true;
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
            if (safetyCounter > 50) break;
        }
        return false;
    }

    public void MoveDragStack(Vector3 newPos)
    {
        if (dragStack.Count == 0) return;
        Vector3 delta = newPos - dragStack[0].transform.position;
        foreach (var card in dragStack)
            card.transform.position += delta;
    }

    public void MoveWholeStack(Vector3 newPos)
    {
        STACK2D bottom = this;
        int safetyCounter = 0;
        while (bottom.stackBelow != null && safetyCounter < 50)
        {
            bottom = bottom.stackBelow;
            safetyCounter++;
        }

        Vector3 delta = newPos - bottom.transform.position;
        STACK2D current = bottom;
        safetyCounter = 0;
        while (current != null && safetyCounter < 50)
        {
            current.transform.position += delta;
            current = current.stackAbove;
            safetyCounter++;
        }
    }
}

