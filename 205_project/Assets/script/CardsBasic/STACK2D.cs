// STACK2D.cs (修改後)
using UnityEngine;
using System.Collections.Generic;

public class STACK2D : MonoBehaviour
{
    private HoverDrag2D hoverDragScript;
    private CardsBehaviour cardsBehaviour;

    private bool isHovering = false;
    private STACK2D parentStack;
    private List<STACK2D> childStacks = new List<STACK2D>();

    public float stackOffset = 0.1f;

    void Awake()
    {
        hoverDragScript = GetComponent<HoverDrag2D>();
        cardsBehaviour = GetComponent<CardsBehaviour>();
    }

    void OnMouseEnter()
    {
        isHovering = true;
    }

    void OnMouseExit()
    {
        isHovering = false;
    }

    /// <summary>
    /// 公開方法，用於檢查此卡牌當前是否被另一張拖拽的卡牌懸停
    /// </summary>
    public bool IsCurrentlyHovered()
    {
        return isHovering;
    }

    public bool OnEndDrag()
    {
        STACK2D[] allStacks = FindObjectsByType<STACK2D>(FindObjectsSortMode.None);

        foreach (STACK2D otherStack in allStacks)
        {
            if (otherStack != this && otherStack.isHovering && CanStackOn(otherStack))
            {
                StackOn(otherStack);
                return true; // 堆疊成功
            }
        }
        return false; // 沒有找到可以堆疊的對象
    }

    // --- 核心修改點 1 ---
    private bool CanStackOn(STACK2D otherStack)
    {
        // 不再检查卡牌名称，只保留不能堆叠在自己子集上的逻辑
        // 这将允许任何不同名的卡牌进行堆叠
        return !otherStack.IsChildOf(this);
    }

    // --- 核心修改點 2 ---
    // 将此方法从 private 改为 public，以便 CardsBehaviour 可以调用
    public void StackOn(STACK2D newParentStack)
    {
        if (parentStack != null)
        {
            parentStack.childStacks.Remove(this);
        }

        transform.SetParent(newParentStack.transform);
        parentStack = newParentStack;
        newParentStack.childStacks.Add(this);

        UpdateStackedPositionAndOrder();
        newParentStack.ReorganizeStackOrders();
    }

    public void UpdateStackedPositionAndOrder()
    {
        if (parentStack != null)
        {
            Vector3 newPosition = parentStack.transform.position + new Vector3(0, -stackOffset, 0);
            transform.position = newPosition;

            SpriteRenderer artworkRenderer = cardsBehaviour.GetArtworkRenderer();
            SpriteRenderer parentArtworkRenderer = parentStack.cardsBehaviour.GetArtworkRenderer();
            if (artworkRenderer != null && parentArtworkRenderer != null)
            {
                artworkRenderer.sortingOrder = parentArtworkRenderer.sortingOrder + 1;
            }
        }
    }

    private void ReorganizeStackOrders()
    {
        STACK2D topParent = this;
        while (topParent.parentStack != null)
        {
            topParent = topParent.parentStack;
        }
        topParent.ApplySortingOrderRecursively(topParent.cardsBehaviour.GetArtworkRenderer().sortingOrder);
    }

    private void ApplySortingOrderRecursively(int baseOrder)
    {
        cardsBehaviour.GetArtworkRenderer().sortingOrder = baseOrder;
        hoverDragScript.SetNewOriginalOrder(baseOrder);

        for (int i = 0; i < childStacks.Count; i++)
        {
            childStacks[i].ApplySortingOrderRecursively(baseOrder + i + 1);
        }
    }

    private bool IsChildOf(STACK2D potentialParent)
    {
        STACK2D current = this.parentStack;
        while (current != null)
        {
            if (current == potentialParent)
            {
                return true;
            }
            current = current.parentStack;
        }
        return false;
    }
}