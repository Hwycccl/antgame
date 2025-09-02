// CardsBehaviour.cs (最终修改版)
using UnityEngine;
using System.Linq;

public class CardsBehaviour : MonoBehaviour
{
    [Header("卡牌數據")]
    [SerializeField] private CardsBasicData cardData;

    [Header("顯示組件")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    private Vector3 originalPosition;
    private Transform originalParent;

    // 引用其他功能腳本
    private HoverDrag2D hoverDragScript;
    private COMBINE2D combineScript;
    private STACK2D stackScript;

    void Awake()
    {
        // 在 Awake 中獲取所有需要的組件引用
        hoverDragScript = GetComponent<HoverDrag2D>();
        combineScript = GetComponent<COMBINE2D>();
        stackScript = GetComponent<STACK2D>();
    }

    public void Initialize(CardsBasicData data)
    {
        cardData = data;
        if (artworkRenderer != null && cardData.cardImage != null)
            artworkRenderer.sprite = cardData.cardImage;
    }

    // 當拖拽開始時，由 HoverDrag2D 調用
    public void BeginDrag()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        transform.SetParent(transform.root);
    }

    // --- 核心修改點 開始 ---
    // 當拖拽結束時，由 HoverDrag2D 調用
    public void EndDrag()
    {
        // 1. 檢查我們是否正懸停在任何其他卡牌上
        STACK2D hoveredStack = FindHoveredStack();

        if (hoveredStack == null)
        {
            // --- 情況 A: 不在任何卡牌上 ---
            // 允許卡牌停留在新的位置，並更新其“原始”位置信息
            originalPosition = transform.position;
            originalParent = transform.parent;

            // 恢復原始的渲染層級
            if (hoverDragScript != null)
            {
                hoverDragScript.ResetSortingOrder();
            }
        }
        else
        {
            // --- 情況 B: 正在懸停在某張卡牌上 ---
            // 嘗試進行堆疊。stackScript.OnEndDrag() 會自動處理能否堆疊的判斷
            if (stackScript != null && stackScript.OnEndDrag())
            {
                // 如果堆疊成功，立即嘗試進行合成檢測
                if (combineScript != null)
                {
                    combineScript.TryToCombineWithNearbyCards();
                }
                // 堆疊成功，流程結束
                return;
            }
            else
            {
                // 如果我們懸停在一張卡上，但堆疊失敗（例如卡牌名稱不同）
                // 則將卡牌送回它原來的位置
                ReturnToOriginalPosition();
            }
        }
    }

    /// <summary>
    /// 查找場景中是否有被當前鼠標懸停的卡牌
    /// </summary>
    private STACK2D FindHoveredStack()
    {
        // 查找場景中所有的 STACK2D 組件
        var allStacks = FindObjectsByType<STACK2D>(FindObjectsSortMode.None);

        // 遍歷並返回第一個處於“被懸停”狀態的卡牌
        return allStacks.FirstOrDefault(stack => stack != this.stackScript && stack.IsCurrentlyHovered());
    }

    /// <summary>
    /// 將卡牌送回原來的位置和狀態
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;

        if (hoverDragScript != null)
        {
            hoverDragScript.ResetSortingOrder();
        }
    }
    // --- 核心修改點 結束 ---

    public CardsBasicData GetCardData()
    {
        return cardData;
    }

    public SpriteRenderer GetArtworkRenderer()
    {
        return artworkRenderer;
    }
}