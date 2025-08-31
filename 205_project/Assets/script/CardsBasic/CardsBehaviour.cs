// CardsBehaviour.cs 
using UnityEngine;

public class CardsBehaviour : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private CardsBasicData cardData;

    [Header("��ʾ���")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    // ���µ���Ϸģʽ�£����ǲ�����Ҫ is"InHand" �������ֵ��
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

    // SetInHand ����Ҳ������Ҫ
    // public void SetInHand(bool inHand) { ... }

    public void OnClick()
    {
        if (cardData != null)
        {
            Debug.Log($"�������: {cardData.cardName}");
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
        // --- ���� CS1061 ������ ---
        // ������߼��������ˡ�
        // ���ȣ����ǳ��Խ��кϳɡ�����ϳɳɹ���COMBINE2D �ű����Լ���������ԭ�Ͽ��ơ�
        if (combineScript != null && combineScript.TryToCombineWithNearbyCards())
        {
            // ����ϳ������Ѿ���ʼ�������������ʲô���������ˡ�
            return;
        }

        // ���û���ҵ��ϳɶ��󣬲��ҿ���Ҳû�б��ѵ�������STACK2D�ű�������
        // ��ô���ǾͰѿ����ͻ���ԭ����λ�á�
        // ���й��� "PlayArea" �͵��� "UseCard" �ľɴ��붼�ѱ��Ƴ���

        transform.SetParent(originalParent);
        transform.position = originalPosition;

        if (hoverDragScript != null) hoverDragScript.ResetSortingOrder();
        // --- �������� ---
    }

    public CardsBasicData GetCardData()
    {
        return cardData;
    }
}
