// 放置於: CardDragger.cs (已添加“按下时”的音效)
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Text;

// --- 新增代码 #1: 添加 IPointerDownHandler 接口 ---
// 这个接口让脚本能够响应鼠标按下的事件
public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;
    private CardStacker stacker;

    private CardStacker rootStackerOfDraggedStack;
    private int originalRootSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    // --- 新增代码 #2: 添加音效相关的变量 ---
    [Header("音效设置")]
    [Tooltip("当鼠标在卡牌上按下时播放的音效")]
    [SerializeField] private AudioClip pointerDownSound;

    private AudioSource audioSource;
    // --- 新增代码结束 ---

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
        stacker = GetComponent<CardStacker>();

        // --- 新增代码 #3: 安全地初始化 AudioSource ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // --- 新增代码结束 ---
    }

    // --- 新增代码 #4: 实现 OnPointerDown 方法 ---
    /// <summary>
    /// 当鼠标指针在卡牌上按下的瞬间被调用
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 播放音效
        if (pointerDownSound != null)
        {
            audioSource.PlayOneShot(pointerDownSound);
        }
    }
    // --- 新增代码结束 ---

    // --- 以下所有代码均保持原样，未作任何修改 ---

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateDescription();
    }

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

        UpdateDescription();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootStackerOfDraggedStack != null)
        {
            rootStackerOfDraggedStack.transform.position = GetMouseWorldPos() + offset;
        }
    }

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

        if (DescriptionManager.Instance != null)
        {
            //DescriptionManager.Instance.HideDescription();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void UpdateDescription()
    {
        if (DescriptionManager.Instance == null) return;

        CardStacker root = stacker.GetRoot();
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
            DescriptionManager.Instance.ShowDescription(card.CardData.cardType.ToString(), card.CardData.description);
        }
    }
}