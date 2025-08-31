// CardsBehaviour.cs 
using UnityEngine;

public class CardsBehaviour : MonoBehaviour
{
    [Header("卡牌数据")]
    [SerializeField] private CardsBasicData cardData;

    [Header("显示组件")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    // 在新的游戏模式下，我们不再需要 is"InHand" 这个布尔值了
    // [SerializeField] private bool isInHand = true; 

    private Vector3 originalPosition;
    private Transform originalParent;

    private HoverDrag2D hoverDragScript;
    private COMBINE2D combineScript;

    void Awake()
    {
        hoverDragScript = GetComponent<HoverDrag2D>();
        combineScript = GetComponent<COMBINE2D>();
    }

    public void Initialize(CardsBasicData data)
    {
        cardData = data;

        if (artworkRenderer != null && cardData.cardImage != null)
            artworkRenderer.sprite = cardData.cardImage;
    }

    // SetInHand 方法也不再需要
    // public void SetInHand(bool inHand) { ... }

    public void OnClick()
    {
        if (cardData != null)
        {
            Debug.Log($"点击卡牌: {cardData.cardName}");
        }
    }

    public void BeginDrag()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        transform.SetParent(transform.root);
    }

    public void EndDrag()
    {
        // --- 错误 CS1061 修正点 ---
        // 这里的逻辑被大大简化了。
        // 首先，我们尝试进行合成。如果合成成功，COMBINE2D 脚本会自己负责销毁原料卡牌。
        if (combineScript != null && combineScript.TryToCombineWithNearbyCards())
        {
            // 如果合成流程已经开始，我们在这里就什么都不用做了。
            return;
        }

        // 如果没有找到合成对象，并且卡牌也没有被堆叠（这由STACK2D脚本处理），
        // 那么我们就把卡牌送回它原来的位置。
        // 所有关于 "PlayArea" 和调用 "UseCard" 的旧代码都已被移除。

        transform.SetParent(originalParent);
        transform.position = originalPosition;

        if (hoverDragScript != null) hoverDragScript.ResetSortingOrder();
        // --- 修正结束 ---
    }

    public CardsBasicData GetCardData()
    {
        return cardData;
    }
}
