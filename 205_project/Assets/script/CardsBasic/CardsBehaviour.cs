/// CardsBehaviour.cs (最終簡化版)
using UnityEngine;

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

        // 讓卡牌脫離父物件，這樣它才能自由移動
        transform.SetParent(transform.root);
    }

    // 當拖拽結束時，由 HoverDrag2D 調用
    public void EndDrag()
    {
        // 優先檢查堆疊邏輯
        if (stackScript != null && stackScript.OnEndDrag())
        {
            // 如果堆疊成功，HoverDrag2D會被告知新的渲染層級，這裡直接返回
            return;
        }

        // 其次檢查合成邏輯
        if (combineScript != null && combineScript.TryToCombineWithNearbyCards())
        {
            // 如果合成成功，卡牌會被銷毀，直接返回
            return;
        }

        // 如果都沒有成功，則把卡牌送回它原來的位置
        transform.SetParent(originalParent);
        transform.position = originalPosition;

        // 恢復原始的渲染層級
        if (hoverDragScript != null)
        {
            hoverDragScript.ResetSortingOrder();
        }
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