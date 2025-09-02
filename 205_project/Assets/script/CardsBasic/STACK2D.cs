// STACK2D.cs (優化版)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class STACK2D : MonoBehaviour
{
    private CardsBehaviour cardsBehaviour;

    private bool isHovering = false;

    // --- 核心修改：簡化父子關係追蹤 ---
    public STACK2D ParentStack { get; private set; }
    public List<STACK2D> ChildStacks { get; private set; } = new List<STACK2D>();

    [Tooltip("每張卡牌在堆疊中的垂直偏移量")]
    public float stackOffset = 0.25f; // 加大偏移量以便觀察

    void Awake()
    {
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

    public bool IsCurrentlyHovered()
    {
        return isHovering;
    }

    /// <summary>
    /// 將當前卡牌堆疊到一個新的父級上
    /// </summary>
    public void StackOn(STACK2D newParentStack)
    {
        // 1. 如果已經有父級，先從舊的父級中脫離
        if (ParentStack != null)
        {
            ParentStack.ChildStacks.Remove(this);
            ParentStack.UpdateStackVisuals(); // 更新舊父級的視覺
        }

        // 2. 建立新的父子關係 (邏輯上和物理上)
        transform.SetParent(newParentStack.transform);
        ParentStack = newParentStack;
        if (!newParentStack.ChildStacks.Contains(this))
        {
            newParentStack.ChildStacks.Add(this);
        }

        // 3. 從最頂層的卡牌開始，更新整個堆疊的視覺表現
        GetRootStack().UpdateStackVisuals();
    }

    /// <summary>
    /// 讓卡牌從當前的堆疊中脫離
    /// </summary>
    public void Unstack()
    {
        if (ParentStack == null) return;

        STACK2D oldParent = ParentStack;

        // 1. 清理邏輯關係
        oldParent.ChildStacks.Remove(this);
        ParentStack = null;

        // 2. 脫離物理父級
        transform.SetParent(transform.root);

        // 3. 更新舊堆疊的視覺
        oldParent.GetRootStack().UpdateStackVisuals();
    }

    /// <summary>
    /// 核心更新函數：遞迴更新所有子卡牌的位置和渲染順序
    /// </summary>
    public void UpdateStackVisuals()
    {
        SpriteRenderer sr = cardsBehaviour.GetArtworkRenderer();
        if (sr == null) return;

        // 遍歷所有孩子，設定它們的位置和排序
        for (int i = 0; i < ChildStacks.Count; i++)
        {
            STACK2D child = ChildStacks[i];
            SpriteRenderer childSr = child.cardsBehaviour.GetArtworkRenderer();
            if (childSr == null) continue;

            // 位置：基於父級的位置，並疊加索引帶來的偏移
            float yOffset = (i + 1) * -stackOffset;
            child.transform.position = this.transform.position + new Vector3(0, yOffset, 0);

            // 渲染順序：保證子級永遠在父級之上
            childSr.sortingOrder = sr.sortingOrder + i + 1;

            // 遞迴調用，更新孫子輩卡牌
            child.UpdateStackVisuals();
        }
    }

    /// <summary>
    /// 查找並返回這個堆疊最頂層的父級卡牌
    /// </summary>
    public STACK2D GetRootStack()
    {
        STACK2D current = this;
        while (current.ParentStack != null)
        {
            current = current.ParentStack;
        }
        return current;
    }

    /// <summary>
    /// 獲取整個堆疊的所有卡牌數據 (包含自己)
    /// </summary>
    public List<CardsBasicData> GetCardsDataInStack()
    {
        List<CardsBasicData> allCards = new List<CardsBasicData>();
        CollectCardsDataRecursively(this, allCards);
        return allCards;
    }

    private void CollectCardsDataRecursively(STACK2D stack, List<CardsBasicData> cardList)
    {
        cardList.Add(stack.cardsBehaviour.GetCardData());
        foreach (var child in stack.ChildStacks)
        {
            CollectCardsDataRecursively(child, cardList);
        }
    }
}