// CardsBehaviour.cs
using UnityEngine;
using TMPro;

public class CardsBehaviour : MonoBehaviour
{
    [Header("卡牌数据")]
    [SerializeField] private CardsBasicData cardData;

    [Header("显示组件")]
    [SerializeField] private SpriteRenderer artworkRenderer;   // 卡牌插画
    [SerializeField] private TextMeshPro titleText;            // 卡牌标题
    [SerializeField] private TextMeshPro descriptionText;      // 卡牌效果描述

    [Header("手牌状态")]
    [SerializeField] private bool isInHand = true;

    private Vector3 originalPosition;
    private Transform originalParent;

    private HoverDrag2D hoverDragScript;
    private STACK2D stackScript;

    void Awake()
    {
        hoverDragScript = GetComponent<HoverDrag2D>();
        stackScript = GetComponent<STACK2D>();
    }

    /// <summary>
    /// 初始化卡牌显示和数据
    /// </summary>
    public void Initialize(CardsBasicData data)
    {
        cardData = data;

        if (artworkRenderer != null && cardData.cardImage != null)
            artworkRenderer.sprite = cardData.cardImage;

        if (titleText != null)
            titleText.text = cardData.cardName;

        if (descriptionText != null)
            descriptionText.text = cardData.description;
    }

    /// <summary>
    /// 设置卡牌是否在手牌中
    /// </summary>
    public void SetInHand(bool inHand)
    {
        isInHand = inHand;
    }

    /// <summary>
    /// 点击卡牌（显示详细信息等）
    /// </summary>
    public void OnClick()
    {
        if (cardData != null)
        {
            Debug.Log($"点击卡牌: {cardData.cardName}");
            // TODO: 可以在这里打开卡牌详细信息界面
        }
    }

    /// <summary>
    /// 开始拖拽（由 HoverDrag2D 调用）
    /// </summary>
    public void BeginDrag()
    {
        if (!isInHand) return;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // 让卡牌显示在最上层
        transform.SetParent(transform.root);
    }

    /// <summary>
    /// 结束拖拽（由 HoverDrag2D 调用）
    /// </summary>
    public void EndDrag()
    {
        if (!isInHand) return;

        bool usedCard = false;

        // 检测鼠标释放位置是否在可放置区域
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
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

            // 重置渲染顺序
            if (hoverDragScript != null) hoverDragScript.ResetSortingOrder();
        }
        else
        {
            // 使用成功销毁卡牌
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
}
