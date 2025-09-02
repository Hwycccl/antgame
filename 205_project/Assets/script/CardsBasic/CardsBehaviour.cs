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

        if (hoveredStack != null)
        {
            // --- 情況 A: 正在懸停在某張卡牌上 ---

            // 1. 直接进行堆叠
            if (stackScript != null)
            {
                // 注意：确保 STACK2D.cs 中的 StackOn 方法是 public
                stackScript.StackOn(hoveredStack);
            }

            // 2. 堆叠后，立即尝试进行合成检测
            if (combineScript != null)
            {
                combineScript.TryToCombineWithNearbyCards();
            }
        }
        else
        {
            // --- 情況 B: 不在任何卡牌上 ---
            // 允许卡牌停留在新的位置，并更新其“原始”位置信息
            originalPosition = transform.position;
            originalParent = transform.parent;

            // 恢复原始的渲染层级
            if (hoverDragScript != null)
            {
                hoverDragScript.ResetSortingOrder();
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