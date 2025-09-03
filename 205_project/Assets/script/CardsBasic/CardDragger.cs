// �����: CardDragger.cs (�����ƶ���ק�Ӽ���)
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoordinate;

    private Card card;

    // �҂���Ҫһ�����Á�ӛס�������Ӽ��ĸ�����
    private CardStacker rootStackerOfDraggedStack;
    private int originalRootSortingOrder;
    [SerializeField] private int dragSortingOrder = 1000;

    void Awake()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        card.Stacker.OnBeginDrag();

        zCoordinate = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();

        // --- �����޸��c �_ʼ ---

        // 1. �ҵ����τ��ƶѵĸ����� (Root)
        rootStackerOfDraggedStack = card.Stacker.GetRoot();

        // 2. ֻ�@ȡ�K�޸ĸ����Ƶ� SpriteRenderer
        var rootRenderer = rootStackerOfDraggedStack.GetComponent<Card>().GetArtworkRenderer();
        if (rootRenderer != null)
        {
            // 3. ӛ䛁K���������Ƶ���Ⱦ�Ӽ�
            originalRootSortingOrder = rootRenderer.sortingOrder;
            rootRenderer.sortingOrder = dragSortingOrder;

            // 4. ���̸��������ƶѵ�ҕ�XЧ��
            // �@��׌�����ӿ��ƵČӼ��������µĸ����ƌӼ��M��ˢ��
            rootStackerOfDraggedStack.UpdateStackVisuals();
        }

        // --- �����޸��c �Y�� ---
    }

    void OnMouseDrag()
    {
        // ���τӕr���҂��Ƅӵ������������Ƶ� Transform
        // ����ӿ��ƶ�����������������ԕ�����һ���Ƅ�
        rootStackerOfDraggedStack.transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        // --- ߀ԭ��Ⱦ�Ӽ����޸� ---
        if (rootStackerOfDraggedStack != null)
        {
            var rootRenderer = rootStackerOfDraggedStack.GetComponent<Card>().GetArtworkRenderer();
            if (rootRenderer != null)
            {
                // 1. �������ƵČӼ�߀ԭ
                rootRenderer.sortingOrder = originalRootSortingOrder;

                // 2. �ٴθ��������ƶѵ�ҕ�X��׌�����ӿ��ƵČӼ�Ҳ߀ԭ
                rootStackerOfDraggedStack.UpdateStackVisuals();
            }
        }

        // --- ���m�ѯB߉݋��׃ ---
        card.Stacker.OnEndDrag();

        // ��������
        rootStackerOfDraggedStack = null;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // �@�������F���� UpdateStackVisuals �Ԅӹ����������Է��fһ
    public void SetOriginalSortingOrder(int newOrder)
    {
        // originalRootSortingOrder = newOrder;
    }
}