using UnityEngine;

public class STACK2D : MonoBehaviour
{
    [Header("检测参数")]
    public float detectRadius = 1.5f;
    public LayerMask stackableLayer;

    [Header("堆叠参数")]
    public Vector3 stackOffset = new Vector3(0, -0.2f, 0);

    [Header("标签验证（多个有效标签）")]
    public string[] validTags = new string[] { "ants" };

    [Header("边框显示")]
    public GameObject borderObject;

    private GameObject nearestStackTarget = null;
    private bool isDragging = false;
    private Vector3 dragOffset;

    void Start()
    {
        if (borderObject != null)
            borderObject.SetActive(false);
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;
            transform.position = mouseWorldPos + dragOffset;

            CheckNearbyObjects();
        }
    }

    public void StartDrag()
    {
        isDragging = true;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;
    }

    public void EndDrag()
    {
        isDragging = false;

        if (nearestStackTarget != null)
        {
            StackOnTarget(nearestStackTarget);
        }

        nearestStackTarget = null;
        if (borderObject != null)
            borderObject.SetActive(false);
    }

    void CheckNearbyObjects()
    {
        nearestStackTarget = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, stackableLayer);

        float minDist = float.MaxValue;
        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            bool tagMatch = false;
            foreach (string validTag in validTags)
            {
                if (hit.gameObject.tag == validTag)
                {
                    tagMatch = true;
                    break;
                }
            }
            if (!tagMatch) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestStackTarget = hit.gameObject;
            }
        }

        if (nearestStackTarget != null && borderObject != null)
            borderObject.SetActive(true);
        else if (borderObject != null)
            borderObject.SetActive(false);
    }

    void StackOnTarget(GameObject target)
    {
        Vector3 targetPos = target.transform.position;
        transform.position = targetPos + stackOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
