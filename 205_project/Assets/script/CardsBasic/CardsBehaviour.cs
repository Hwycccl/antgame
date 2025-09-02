// CardsBehaviour.cs (�����޸İ�)
using UnityEngine;
using System.Linq;

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
        transform.SetParent(transform.root);
    }

    // --- �����޸��c �_ʼ ---
    // ����ק�Y���r���� HoverDrag2D �{��
    public void EndDrag()
    {
        // 1. �z���҂��Ƿ�����ͣ���κ�����������
        STACK2D hoveredStack = FindHoveredStack();

        if (hoveredStack != null)
        {
            // --- ��r A: ���ڑ�ͣ��ĳ�������� ---

            // 1. ֱ�ӽ��жѵ�
            if (stackScript != null)
            {
                // ע�⣺ȷ�� STACK2D.cs �е� StackOn ������ public
                stackScript.StackOn(hoveredStack);
            }

            // 2. �ѵ����������Խ��кϳɼ��
            if (combineScript != null)
            {
                combineScript.TryToCombineWithNearbyCards();
            }
        }
        else
        {
            // --- ��r B: �����κο����� ---
            // ������ͣ�����µ�λ�ã��������䡰ԭʼ��λ����Ϣ
            originalPosition = transform.position;
            originalParent = transform.parent;

            // �ָ�ԭʼ����Ⱦ�㼶
            if (hoverDragScript != null)
            {
                hoverDragScript.ResetSortingOrder();
            }
        }
    }

    /// <summary>
    /// ���҈������Ƿ��б���ǰ��ˑ�ͣ�Ŀ���
    /// </summary>
    private STACK2D FindHoveredStack()
    {
        // ���҈��������е� STACK2D �M��
        var allStacks = FindObjectsByType<STACK2D>(FindObjectsSortMode.None);

        // ��v�K���ص�һ��̎춡�����ͣ����B�Ŀ���
        return allStacks.FirstOrDefault(stack => stack != this.stackScript && stack.IsCurrentlyHovered());
    }

    /// <summary>
    /// �������ͻ�ԭ���λ�ú͠�B
    /// </summary>
    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;

        if (hoverDragScript != null)
        {
            hoverDragScript.ResetSortingOrder();
        }
    }
    // --- �����޸��c �Y�� ---

    public CardsBasicData GetCardData()
    {
        return cardData;
    }

    public SpriteRenderer GetArtworkRenderer()
    {
        return artworkRenderer;
    }
}