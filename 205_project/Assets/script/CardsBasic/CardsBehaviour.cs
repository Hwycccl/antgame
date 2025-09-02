// CardsBehaviour.cs (΢�{��)
using UnityEngine;
using System.Linq;

public class CardsBehaviour : MonoBehaviour
{
    [Header("���Ɣ���")]
    [SerializeField] private CardsBasicData cardData;

    [Header("�@ʾ�M��")]
    [SerializeField] private SpriteRenderer artworkRenderer;

    private Vector3 originalPosition;
    // ������Ҫ originalParent������҂�����Ó�x�� root
    // private Transform originalParent; 

    // �������������_��
    private HoverDrag2D hoverDragScript;
    private COMBINE2D combineScript;
    private STACK2D stackScript;

    void Awake()
    {
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

    public void BeginDrag()
    {
        originalPosition = transform.position;

        // --- �����޸��c������ Unstack ---
        if (stackScript != null)
        {
            stackScript.Unstack();
        }
    }

    public void EndDrag()
    {
        STACK2D hoveredStack = FindHoveredStack();

        if (hoveredStack != null)
        {
            // ��r A: ���ڑ�ͣ��ĳ��������
            if (stackScript != null)
            {
                stackScript.StackOn(hoveredStack);
            }

            if (combineScript != null)
            {
                // ע�⣺�_�� combineScript ʹ�õ��� GetRootStack() ��@ȡ�����ѯB�Ŀ���
                combineScript.TryToCombineWithNearbyCards(hoveredStack.GetRootStack());
            }
        }
        else
        {
            // ��r B: �����κο����ϣ����Sͣ������λ��
            // ����Ҫ���~����£���� Unstack �ѽ�̎����Ó�x
            if (hoverDragScript != null)
            {
                hoverDragScript.ResetSortingOrder();
            }
        }
    }

    private STACK2D FindHoveredStack()
    {
        var allStacks = FindObjectsByType<STACK2D>(FindObjectsSortMode.None);
        return allStacks.FirstOrDefault(stack => stack != this.stackScript && stack.IsCurrentlyHovered());
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