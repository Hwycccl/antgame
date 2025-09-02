// �����: CardDragger.cs (��K�\���)
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;
    private SpriteRenderer artworkRenderer;

    private int originalSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // OnMouseDown �ѽ��_�J�������ģ������Ƴ��@�e�����I
        if (mainCamera == null) { Debug.LogError("Main Camera is not found!"); return; }
        zCoordinate = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
        artworkRenderer = card.GetArtworkRenderer();
        if (artworkRenderer != null)
        {
            originalSortingOrder = artworkRenderer.sortingOrder;
            artworkRenderer.sortingOrder = dragSortingOrder;
        }
        card.Stacker.OnBeginDrag();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        Debug.Log($"[{gameObject.name}] OnMouseUp: Step 1 - Mouse button released.");
        if (artworkRenderer != null)
        {
            artworkRenderer.sortingOrder = originalSortingOrder;
        }

        Debug.Log($"[{gameObject.name}] OnMouseUp: Step 2 - Now calling OnEndDrag(). If the game freezes, the problem is inside CardStacker's logic.");

        // ���жѯB߉݋�������AӋ���l�����@�e
        card.Stacker.OnEndDrag();

        // ������ܿ��������@�l���I������]�п���
        Debug.Log($"[{gameObject.name}] OnMouseUp: Step 3 - OnEndDrag() FINISHED SUCCESSFULLY. No freeze occurred.");
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void SetOriginalSortingOrder(int newOrder)
    {
        originalSortingOrder = newOrder;
    }
}