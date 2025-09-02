// CardsBehaviour.cs (微調版)
using UnityEngine;
using System.Linq;

public class CardsBehaviour : MonoBehaviour
{
    [Header("卡牌數據")]
    [SerializeField] private CardsBasicData cardData;

    [Header("顯示組件")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    private Vector3 originalPosition;
    // 不再需要 originalParent，因為我們總是脫離到 root
    // private Transform originalParent; 

    // 引用其他功能腳本
    private HoverDrag2D hoverDragScript;
    private COMBINE2D combineScript;
    private STACK2D stackScript;

    void Awake()
    {
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

    public void BeginDrag()
    {
        originalPosition = transform.position;

        // --- 核心修改點：呼叫 Unstack ---
        if (stackScript != null)
        {
            stackScript.Unstack();
        }
    }

    public void EndDrag()
    {
        STACK2D hoveredStack = FindHoveredStack();

        if (hoveredStack != null)
        {
            // 情況 A: 正在懸停在某張卡牌上
            if (stackScript != null)
            {
                stackScript.StackOn(hoveredStack);
            }

            if (combineScript != null)
            {
                // 注意：確保 combineScript 使用的是 GetRootStack() 來獲取整個堆疊的卡牌
                combineScript.TryToCombineWithNearbyCards(hoveredStack.GetRootStack());
            }
        }
        else
        {
            // 情況 B: 不在任何卡牌上，允許停留在新位置
            // 不需要做額外的事，因為 Unstack 已經處理了脫離
            if (hoverDragScript != null)
            {
                hoverDragScript.ResetSortingOrder();
            }
        }
    }

    private STACK2D FindHoveredStack()
    {
        var allStacks = FindObjectsByType<STACK2D>(FindObjectsSortMode.None);
        return allStacks.FirstOrDefault(stack => stack != this.stackScript && stack.IsCurrentlyHovered());
    }

    public CardsBasicData GetCardData()
    {
        return cardData;
    }

    public SpriteRenderer GetArtworkRenderer()
    {
        return artworkRenderer;
    }
}