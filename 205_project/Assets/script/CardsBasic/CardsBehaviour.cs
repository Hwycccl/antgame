//CardsBehavior
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ������Ϊ�ű��������ڿ�����Ϸ������
/// </summary>
public class CardsBehaviour : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CardsBasicData cardData;
    [SerializeField] private SpriteRenderer cardSpriteRenderer;
    [SerializeField] private bool isInHand = true;

    private Vector3 originalPosition;
    private Transform originalParent;

    /// <summary>
    /// ��ʼ������
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
            // �������߼�
            Debug.Log($"�������: {cardData.cardName}");
            // ������������ʾ������ϸ��Ϣ
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInHand) return;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // ʹ�������϶�ʱ��ʾ�����ϲ�
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInHand) return;

        // ��������ƶ�
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        newPosition.z = 0;
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInHand) return;

        // ����Ƿ��������Ч����
        bool usedCard = false;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(eventData.position), Vector2.zero);

        if (hit.collider != null)
        {
            // ����Ƿ��������Ϸ����
            if (hit.collider.CompareTag("PlayArea"))
            {
                // ����ʹ�ÿ���
                usedCard = CardsManager.Instance.UseCard(cardData);
            }
        }

        if (!usedCard)
        {
            // ����ԭλ��
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
        else
        {
            // ����ʹ�óɹ���������Ϸ����
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    public CardsBasicData GetCardData()
    {
        return cardData;
    }

    /// <summary>
    /// ���ÿ����Ƿ���������
    /// </summary>
    public void SetInHand(bool inHand)
    {
        isInHand = inHand;
    }
}