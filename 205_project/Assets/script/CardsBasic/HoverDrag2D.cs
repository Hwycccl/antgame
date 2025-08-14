using UnityEngine;

public class HoverDrag2D : MonoBehaviour
{
    public float hoverScaleFactor = 0.85f;
    public float pressScaleFactor = 0.7f;
    public float lerpSpeed = 5f;
    public float hoverCooldownTime = 1f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isDragging = false;
    private Vector3 dragOffset;

    private bool hoverCooldown = false;
    private float hoverCooldownTimer = 0f;
    private STACK2D stackScript;

    private static HoverDrag2D currentlyDragging = null;

    private bool isHovering = false;  // �����ֶ�������ͣ״̬

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        stackScript = GetComponent<STACK2D>();
    }

    void Update()
    {
        if (hoverCooldown)
        {
            hoverCooldownTimer -= Time.deltaTime;
            if (hoverCooldownTimer <= 0f)
                hoverCooldown = false;
        }

        // ͨ�����߼��ȷ���Ƿ���ͣ�Լ�
        if (!isDragging && !hoverCooldown)
        {
            CheckHover();
        }
        else
        {
            // �϶�ʱ�϶�������ͣ
            isHovering = false;
        }

        Vector3 desiredScale = originalScale;

        if (isDragging)
        {
            desiredScale = originalScale * pressScaleFactor;
        }
        else if (isHovering)
        {
            desiredScale = originalScale * hoverScaleFactor;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * lerpSpeed);

        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;
            transform.position = mouseWorldPos + dragOffset;
        }
    }

    // ʹ�����߼���ж��Ƿ������ͣ���Լ�����
    private void CheckHover()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (!isHovering)
            {
                // ֮ǰû����ͣ��������ͣ����������״̬
                isHovering = true;
            }
        }
        else
        {
            if (isHovering)
            {
                // ֮ǰ��ͣ�����ڲ���ͣ�������뿪״̬
                isHovering = false;
            }
        }
    }

    void OnMouseDown()
    {
        targetScale = originalScale * pressScaleFactor;
        isDragging = true;

        currentlyDragging = this;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

        if (stackScript != null)
            stackScript.StartDrag();
    }

    void OnMouseUp()
    {
        isDragging = false;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && col.OverlapPoint(mouseWorldPos))
            targetScale = originalScale * hoverScaleFactor;
        else
            targetScale = originalScale;

        hoverCooldown = true;
        hoverCooldownTimer = hoverCooldownTime;

        currentlyDragging = null;

        if (stackScript != null)
            stackScript.EndDrag();
    }
}
