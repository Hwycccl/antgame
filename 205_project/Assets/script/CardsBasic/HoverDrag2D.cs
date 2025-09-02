// HoverDrag2D.cs (�Ƽ�������������������հ汾)
using UnityEngine;

[RequireComponent(typeof(CardsBehaviour))]
public class HoverDrag2D : MonoBehaviour
{
    private CardsBehaviour cardsBehaviour;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private Vector3 offset;
    private int originalSortingOrder;

    private float distanceToCamera; // ���ڴ洢��ק��ʼʱ�Ĺ̶����������

    [Header("��ק�r��������Ⱦ�Ӽ�")]
    [Tooltip("��ק�r�������Ƶ� Order in Layer �������@��ֵ���_���������ό�")]
    public int sortingOrderOnDrag = 100;

    void Awake()
    {
        cardsBehaviour = GetComponent<CardsBehaviour>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        spriteRenderer = cardsBehaviour.GetArtworkRenderer();
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    void OnMouseDown()
    {
        // 1. ����ק��ʼʱ������һ�ο���ƽ�浽������ľ��벢�洢
        //    �����������������ȷ��������ת�����ȶ���
        distanceToCamera = mainCamera.WorldToScreenPoint(transform.position).z;

        // 2. ���������λ���뿨�����ĵ�ƫ����
        offset = transform.position - GetMouseWorldPos();

        // 3. ������Ⱦ�㼶���ÿ�����ʾ��������
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrderOnDrag;
        }

        // 4. ֪ͨ CardsBehaviour ��ק�ѿ�ʼ
        if (cardsBehaviour != null)
        {
            cardsBehaviour.BeginDrag();
        }
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        if (cardsBehaviour != null)
        {
            cardsBehaviour.EndDrag();
        }
    }

    // ��������Ļ����ת��Ϊ��������
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        // Z ���ֵʹ����ק��ʼʱ�洢�Ĺ̶�����
        mousePoint.z = distanceToCamera;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void ResetSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    public void SetNewOriginalOrder(int newOrder)
    {
        originalSortingOrder = newOrder;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = newOrder;
        }
    }
}