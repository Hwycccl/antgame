/// CardsBehaviour.cs (��K������)
using UnityEngine;

public class CardsBehaviour : MonoBehaviour
{
    [Header("���Ɣ���")]
    [SerializeField] private CardsBasicData cardData;

    [Header("�@ʾ�M��")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    private Vector3 originalPosition;
    private Transform originalParent;

    // �������������_��
    private HoverDrag2D hoverDragScript;
    private COMBINE2D combineScript;
    private STACK2D stackScript;

    void Awake()
    {
        // �� Awake �Ы@ȡ������Ҫ�ĽM������
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

    // ����ק�_ʼ�r���� HoverDrag2D �{��
    public void BeginDrag()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        // ׌����Ó�x��������@�������������Ƅ�
        transform.SetParent(transform.root);
    }

    // ����ק�Y���r���� HoverDrag2D �{��
    public void EndDrag()
    {
        // ���șz��ѯB߉݋
        if (stackScript != null && stackScript.OnEndDrag())
        {
            // ����ѯB�ɹ���HoverDrag2D������֪�µ���Ⱦ�Ӽ����@�eֱ�ӷ���
            return;
        }

        // ��Ιz��ϳ�߉݋
        if (combineScript != null && combineScript.TryToCombineWithNearbyCards())
        {
            // ����ϳɳɹ������ƕ����N����ֱ�ӷ���
            return;
        }

        // ������]�гɹ����t�ѿ����ͻ���ԭ���λ��
        transform.SetParent(originalParent);
        transform.position = originalPosition;

        // �֏�ԭʼ����Ⱦ�Ӽ�
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