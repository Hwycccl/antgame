// STACK2D.cs (修正並優化後的最終版本)
using UnityEngine;
using System.Collections.Generic;

public class STACK2D : MonoBehaviour
{
    private CardsBehaviour cardsBehaviour;
    private bool isHovering = false;

    // 使用屬性來清晰地管理父子關係
    public STACK2D ParentStack { get; private set; }
    public List<STACK2D> ChildStacks { get; private set; } = new List<STACK2D>();

    [Header("堆疊設定")]
    [Tooltip("每張卡牌在堆疊中的垂直偏移量")]
    public float stackOffset = 0.25f; // 您可以在 Unity 編輯器中調整此值

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

    // 加入新的堆疊
    public void StackOn(STACK2D newParentStack)
    {
        if (ParentStack != null)
        {
            ParentStack.ChildStacks.Remove(this);
            ParentStack.UpdateStackVisuals();
        }

        transform.SetParent(newParentStack.transform);
        ParentStack = newParentStack;
        if (!newParentStack.ChildStacks.Contains(this))
        {
            newParentStack.ChildStacks.Add(this);
        }

        GetRootStack().UpdateStackVisuals();
    }

    // 從堆疊中脫離
    public void Unstack()
    {
        if (ParentStack == null) return;
        STACK2D oldParent = ParentStack;
        oldParent.ChildStacks.Remove(this);
        ParentStack = null;
        transform.SetParent(transform.root);
        oldParent.GetRootStack().UpdateStackVisuals();
    }

    /// <summary>
    /// 核心修正：遞迴更新所有子卡牌的位置和渲染順序
    /// </summary>
    public void UpdateStackVisuals()
    {
        SpriteRenderer sr = cardsBehaviour.GetArtworkRenderer();
        if (sr == null) return;

        // 遍歷所有孩子，根據它們的順序（索引 i）來設定位置
        for (int i = 0; i < ChildStacks.Count; i++)
        {
            STACK2D child = ChildStacks[i];
            SpriteRenderer childSr = child.cardsBehaviour.GetArtworkRenderer();
            if (childSr == null) continue;

            // --- 這就是解決重疊問題的關鍵邏輯 ---
            // 每個子級的位置 = 父級位置 + (子級索引 + 1) * 偏移量
            float yOffset = (i + 1) * -stackOffset;
            child.transform.position = this.transform.position + new Vector3(0, yOffset, 0);

            childSr.sortingOrder = sr.sortingOrder + i + 1;

            // 遞迴調用，更新更深層次的卡牌
            child.UpdateStackVisuals();
        }
    }

    // 找到最頂層的父級
    public STACK2D GetRootStack()
    {
        STACK2D current = this;
        while (current.ParentStack != null)
        {
            current = current.ParentStack;
        }
        return current;
    }

    // 獲取堆疊中所有卡牌數據
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