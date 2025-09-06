// 放置於: CardDragger.cs (已修正错误)
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Text;

// 使用新的事件接口来统一处理所有输入
public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;
    private CardStacker stacker;

    private CardStacker rootStackerOfDraggedStack;
    private int originalRootSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
        stacker = GetComponent<CardStacker>();
    }

    // --- 新增功能：点击时显示描述 ---
    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateDescription();
    }

    // --- 这里是原 OnMouseDown() 的逻辑 ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        card.Stacker.OnBeginDrag();

        zCoordinate = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();

        rootStackerOfDraggedStack = card.Stacker.GetRoot();

        var rootRenderer = rootStackerOfDraggedStack.GetComponent<Card>().GetArtworkRenderer();
        if (rootRenderer != null)
        {
            originalRootSortingOrder = rootRenderer.sortingOrder;
            rootRenderer.sortingOrder = dragSortingOrder;
            rootStackerOfDraggedStack.UpdateStackVisuals();
        }

        // 开始拖动时，也更新一次描述
        UpdateDescription();
    }

    // --- 这里是原 OnMouseDrag() 的逻辑 ---
    public void OnDrag(PointerEventData eventData)
    {
        if (rootStackerOfDraggedStack != null)
        {
            rootStackerOfDraggedStack.transform.position = GetMouseWorldPos() + offset;
        }
    }

    // --- 这里是原 OnMouseUp() 的逻辑 ---
    public void OnEndDrag(PointerEventData eventData)
    {
        if (rootStackerOfDraggedStack != null)
        {
            var rootRenderer = rootStackerOfDraggedStack.GetComponent<Card>().GetArtworkRenderer();
            if (rootRenderer != null)
            {
                rootRenderer.sortingOrder = originalRootSortingOrder;
                rootStackerOfDraggedStack.UpdateStackVisuals();
            }
        }

        card.Stacker.OnEndDrag();
        rootStackerOfDraggedStack = null;

        // --- 新增功能：结束拖动时隐藏描述框 ---
        if (DescriptionManager.Instance != null)
        {
            DescriptionManager.Instance.HideDescription();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // --- 新增功能：更新描述文本框 ---
    private void UpdateDescription()
    {
        if (DescriptionManager.Instance == null) return;

        CardStacker root = stacker.GetRoot();
        // 判断条件改为检查牌堆中的卡牌总数
        if (root.GetCardsInStack().Count > 1)
        {
            CardCombiner combiner = root.GetComponent<CardCombiner>();

            if (combiner != null && combiner.isCombining)
            {
                float remainingTime = combiner.GetRemainingTime();
                DescriptionManager.Instance.ShowDescription("In synthesis.....", $"remaining {remainingTime:F1} mins");
            }
            else
            {
                var cardCounts = root.GetCardsInStack()
                    .GroupBy(c => c.CardData.cardName)
                    .ToDictionary(g => g.Key, g => g.Count());

                StringBuilder description = new StringBuilder();
                foreach (var entry in cardCounts)
                {
                    description.AppendLine($"{entry.Key} ×{entry.Value}");
                }
                DescriptionManager.Instance.ShowDescription("Deck of cards", description.ToString());
            }
        }
        else
        {
            // 单张卡牌
            DescriptionManager.Instance.ShowDescription(card.CardData.cardType.ToString(), card.CardData.description);
        }
    }
}