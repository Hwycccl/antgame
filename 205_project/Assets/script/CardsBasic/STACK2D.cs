// STACK2D.cs
using UnityEngine;
using System.Collections;
using System.Linq;

public class STACK2D : MonoBehaviour
{
    [Header("������")]
    public float detectRadius = 1.5f;
    public LayerMask stackableLayer;

    [Header("�ѵ��붯��")]
    public Vector3 stackOffset = new Vector3(0, -0.2f, 0);
    public float snapSpeed = 10f;

    [Header("��ǩ��֤")]
    public string[] validTags = new string[] { "Unit" };

    [Header("������ʾ")]
    public GameObject borderObject;

    private bool isDragging = false;
    private GameObject nearestStackTarget = null;
    private HoverDrag2D hoverDragScript;
    private SpriteRenderer artworkRenderer;
    private bool isStacked = false;

    public bool IsStacked() => isStacked;

    void Start()
    {
        if (borderObject != null) borderObject.SetActive(false);

        hoverDragScript = GetComponent<HoverDrag2D>();
        if (hoverDragScript != null)
            artworkRenderer = hoverDragScript.artworkRenderer;
    }

    void Update()
    {
        if (isDragging)
        {
            CheckNearbyObjects();
        }
    }

    public void StartDrag()
    {
        isDragging = true;
        isStacked = false;
    }

    public void EndDrag()
    {
        isDragging = false;

        if (borderObject != null)
            borderObject.SetActive(false);

        if (nearestStackTarget != null)
        {
            StartCoroutine(SnapAndStack(nearestStackTarget));
        }
        else
        {
            // û�жѵ�Ŀ�����������
            if (hoverDragScript != null)
                hoverDragScript.ResetSortingOrder();
        }

        nearestStackTarget = null;
    }

    void CheckNearbyObjects()
    {
        nearestStackTarget = null;
        float minDist = float.MaxValue;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, stackableLayer);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;
            if (!validTags.Any(tag => hit.gameObject.CompareTag(tag))) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestStackTarget = hit.gameObject;
            }
        }

        if (borderObject != null)
            borderObject.SetActive(nearestStackTarget != null);
    }

    private IEnumerator SnapAndStack(GameObject target)
    {
        if (hoverDragScript) hoverDragScript.enabled = false;

        // ����Ŀ���Ϸ�
        var targetHover = target.GetComponent<HoverDrag2D>();
        var targetArtwork = targetHover != null ? targetHover.artworkRenderer : null;
        if (artworkRenderer != null && targetArtwork != null)
            artworkRenderer.sortingOrder = targetArtwork.sortingOrder + 1;

        // ����λ��
        Vector3 targetPosition = target.transform.position + stackOffset;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isStacked = true;

        // �ͷ���ק״̬
        if (hoverDragScript) hoverDragScript.enabled = true;
    }
}
