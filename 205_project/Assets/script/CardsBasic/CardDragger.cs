// �����: CardDragger.cs (����ӡ�����ʱ������Ч)
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Text;

// --- �������� #1: ��� IPointerDownHandler �ӿ� ---
// ����ӿ��ýű��ܹ���Ӧ��갴�µ��¼�
public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;
    private CardStacker stacker;

    private CardStacker rootStackerOfDraggedStack;
    private int originalRootSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    // --- �������� #2: �����Ч��صı��� ---
    [Header("��Ч����")]
    [Tooltip("������ڿ����ϰ���ʱ���ŵ���Ч")]
    [SerializeField] private AudioClip pointerDownSound;

    private AudioSource audioSource;
    // --- ����������� ---

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
        stacker = GetComponent<CardStacker>();

        // --- �������� #3: ��ȫ�س�ʼ�� AudioSource ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // --- ����������� ---
    }

    // --- �������� #4: ʵ�� OnPointerDown ���� ---
    /// <summary>
    /// �����ָ���ڿ����ϰ��µ�˲�䱻����
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // ������Ч
        if (pointerDownSound != null)
        {
            audioSource.PlayOneShot(pointerDownSound);
        }
    }
    // --- ����������� ---

    // --- �������д��������ԭ����δ���κ��޸� ---

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateDescription();
    }

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

        UpdateDescription();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootStackerOfDraggedStack != null)
        {
            rootStackerOfDraggedStack.transform.position = GetMouseWorldPos() + offset;
        }
    }

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

        if (DescriptionManager.Instance != null)
        {
            //DescriptionManager.Instance.HideDescription();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void UpdateDescription()
    {
        if (DescriptionManager.Instance == null) return;

        CardStacker root = stacker.GetRoot();
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
            DescriptionManager.Instance.ShowDescription(card.CardData.cardType.ToString(), card.CardData.description);
        }
    }
}