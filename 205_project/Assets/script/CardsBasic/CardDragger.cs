// �����: CardDragger.cs (����������)
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Text;

// ʹ���µ��¼��ӿ���ͳһ������������
public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;
    private CardStacker stacker;

    private CardStacker rootStackerOfDraggedStack;
    private int originalRootSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
        stacker = GetComponent<CardStacker>();
    }

    // --- �������ܣ����ʱ��ʾ���� ---
    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateDescription();
    }

    // --- ������ԭ OnMouseDown() ���߼� ---
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

        // ��ʼ�϶�ʱ��Ҳ����һ������
        UpdateDescription();
    }

    // --- ������ԭ OnMouseDrag() ���߼� ---
    public void OnDrag(PointerEventData eventData)
    {
        if (rootStackerOfDraggedStack != null)
        {
            rootStackerOfDraggedStack.transform.position = GetMouseWorldPos() + offset;
        }
    }

    // --- ������ԭ OnMouseUp() ���߼� ---
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

        // --- �������ܣ������϶�ʱ���������� ---
        if (DescriptionManager.Instance != null)
        {
            DescriptionManager.Instance.HideDescription();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // --- �������ܣ����������ı��� ---
    private void UpdateDescription()
    {
        if (DescriptionManager.Instance == null) return;

        CardStacker root = stacker.GetRoot();
        // �ж�������Ϊ����ƶ��еĿ�������
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
                    description.AppendLine($"{entry.Key} ��{entry.Value}");
                }
                DescriptionManager.Instance.ShowDescription("Deck of cards", description.ToString());
            }
        }
        else
        {
            // ���ſ���
            DescriptionManager.Instance.ShowDescription(card.CardData.cardType.ToString(), card.CardData.description);
        }
    }
}