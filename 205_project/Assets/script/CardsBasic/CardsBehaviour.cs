//CardsBehavior
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 卡牌行为脚本，挂载在卡牌游戏对象上
/// </summary>
public class CardsBehaviour : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CardsBasicData cardData;
    [SerializeField] private SpriteRenderer cardSpriteRenderer;
    [SerializeField] private bool isInHand = true;

    private Vector3 originalPosition;
    private Transform originalParent;

    /// <summary>
    /// 初始化卡牌
    /// </summary>
    public void Initialize(CardsBasicData data)
    {
        cardData = data;
        if (cardSpriteRenderer != null && cardData.cardImage != null)
        {
            cardSpriteRenderer.sprite = cardData.cardImage;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 左键点击逻辑
            Debug.Log($"点击卡牌: {cardData.cardName}");
            // 可以在这里显示卡牌详细信息
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInHand) return;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // 使卡牌在拖动时显示在最上层
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInHand) return;

        // 跟随鼠标移动
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        newPosition.z = 0;
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInHand) return;

        // 检查是否放置在有效区域
        bool usedCard = false;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(eventData.position), Vector2.zero);

        if (hit.collider != null)
        {
            // 检查是否放置在游戏区域
            if (hit.collider.CompareTag("PlayArea"))
            {
                // 尝试使用卡牌
                usedCard = CardsManager.Instance.UseCard(cardData);
            }
        }

        if (!usedCard)
        {
            // 返回原位置
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
        else
        {
            // 卡牌使用成功，销毁游戏对象
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 获取卡牌数据
    /// </summary>
    public CardsBasicData GetCardData()
    {
        return cardData;
    }

    /// <summary>
    /// 设置卡牌是否在手牌中
    /// </summary>
    public void SetInHand(bool inHand)
    {
        isInHand = inHand;
    }
}