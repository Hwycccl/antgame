// HoverDrag2D.cs (��K������)
using UnityEngine;

[RequireComponent(typeof(CardsBehaviour))]
public class HoverDrag2D : MonoBehaviour
{
    private CardsBehaviour cardsBehaviour;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private Vector3 offset;
    private int originalSortingOrder;

    [Header("��ק�r��������Ⱦ�Ӽ�")]
    [Tooltip("��ק�r�������Ƶ� Order in Layer �������@��ֵ���_���������ό�")]
    public int sortingOrderOnDrag = 100;

    void Awake()
    {
        cardsBehaviour = GetComponent<CardsBehaviour>();
        mainCamera = Camera.main; // �@ȡ���zӰ�C������
    }

    void Start()
    {
        // �� CardsBehaviour �@ȡ SpriteRenderer
        spriteRenderer = cardsBehaviour.GetArtworkRenderer();
        if (spriteRenderer != null)
        {
            // ����ԭʼ����Ⱦ���
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    void OnMouseDown()
    {
        // --- ��ק�_ʼ ---
        // 1. Ӌ�㻬���c��λ���c�������ĵ�ƫ����
        offset = transform.position - GetMouseWorldPos();

        // 2. ������Ⱦ�Ӽ���׌�����@ʾ��������
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrderOnDrag;
        }

        // 3. ֪ͨ CardsBehaviour ��ק���_ʼ
        if (cardsBehaviour != null)
        {
            cardsBehaviour.BeginDrag();
        }
    }

    void OnMouseDrag()
    {
        // --- ��ק�^���� ---
        // ���m���¿��Ƶ�λ�ã�ʹ����S���󣨁K����ƫ������
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        // --- ��ק�Y�� ---
        // ֪ͨ CardsBehaviour ��ק�ѽY����׌��̎��ѯB���ϳɻ�wλ��߉݋
        if (cardsBehaviour != null)
        {
            cardsBehaviour.EndDrag();
        }
    }

    // �������ΞĻ�����D�Q����������
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        // Z �S��ֵ��Ҫ�O����zӰ�C�����w�ľ��x
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // ���������������߉݋̎���ᣨ��wλ���֏�ԭʼ����Ⱦ���
    public void ResetSortingOrder()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
        }
    }

    // �������������S�ⲿ�_������STACK2D���ڶѯB����´˿�Ƭ�ġ�ԭʼ����Ⱦ���
    public void SetNewOriginalOrder(int newOrder)
    {
        originalSortingOrder = newOrder;
        // ͬ�rҲ���®�ǰ����Ⱦ���������ѽ��ѯB����
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = newOrder;
        }
    }
}
