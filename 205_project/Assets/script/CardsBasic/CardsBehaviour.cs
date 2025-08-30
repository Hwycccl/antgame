// CardsBehaviour.cs
using UnityEngine;

public class CardsBehaviour : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private CardsBasicData cardData;

    [Header("��ʾ���")]
    [SerializeField] private SpriteRenderer artworkRenderer;   // ���Ʋ廭

    [Header("����״̬")]
    [SerializeField] private bool isInHand = true;

    private Vector3 originalPosition;
    private Transform originalParent;

    private HoverDrag2D hoverDragScript;
    private COMBINE2D combineScript;

    void Awake()
    {
        hoverDragScript = GetComponent<HoverDrag2D>();
        combineScript = GetComponent<COMBINE2D>();
    }

    /// <summary>
    /// ��ʼ��������ʾ������
    /// </summary>
    public void Initialize(CardsBasicData data)
    {
        cardData = data;

        if (artworkRenderer != null && cardData.cardImage != null)
            artworkRenderer.sprite = cardData.cardImage;
    }

    /// <summary>
    /// ���ÿ����Ƿ���������
    /// </summary>
    public void SetInHand(bool inHand)
    {
        isInHand = inHand;
    }

    /// <summary>
    /// ������ƣ���ʾ��ϸ��Ϣ�ȣ�
    /// </summary>
    public void OnClick()
    {
        if (cardData != null)
        {
            Debug.Log($"�������: {cardData.cardName}");
            // TODO: ����������򿪿�����ϸ��Ϣ����
        }
    }

    /// <summary>
    /// ��ʼ��ק���� HoverDrag2D ���ã�
    /// </summary>
    public void BeginDrag()
    {
        if (!isInHand) return;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // �ÿ�����ʾ�����ϲ�
        transform.SetParent(transform.root);
    }

    /// <summary>
    /// ������ק���� HoverDrag2D ���ã�
    /// </summary>
    public void EndDrag()
    {
        if (!isInHand) return;

        // ���ȳ����븽���Ŀ��ƽ��кϳ�
        if (combineScript != null && combineScript.TryToCombineWithNearbyCards())
        {
            // ����ɹ��ҵ��˺ϳɶ��󲢿�ʼ�˺ϳ����̣���ֱ�ӷ��أ���ִ�к����Ŀ���ʹ���߼�
            return;
        }

        bool usedCard = false;

        // �������ͷ�λ���Ƿ��ڿɷ�������
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
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

            // ������Ⱦ˳��
            if (hoverDragScript != null) hoverDragScript.ResetSortingOrder();
        }
        else
        {
            // ʹ�óɹ����ٿ���
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
}
