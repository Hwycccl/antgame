
using UnityEngine;

/// <summary>
/// ��һ����UI���������塰ê��������Ļ�Ĺ̶�λ�á�
/// </summary>
public class AnchorToScreen : MonoBehaviour
{
    [Header("�����")]
    [Tooltip("ָ�����ڼ���λ�õ��������")]
    [SerializeField] private Camera mainCamera;

    [Header("ê������")]
    [Tooltip("Ҫê���ı�׼����Ļ���ꡣ(0,0)�����½�, (1,1)�����Ͻ�")]
    [SerializeField] private Vector2 screenAnchorPoint = new Vector2(1, 1);

    [Tooltip("�����������֮��ľ��루Z�ᣩ")]
    [SerializeField] private float distanceFromCamera = 10f;

    void Awake()
    {
        // ���û���ֶ�ָ��������������Զ����ҳ����е��������
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // ʹ��LateUpdate����ȷ�����������������ƶ�����ת֮���ٸ�������λ��
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            Debug.LogError("û���ҵ���ָ���������");
            return;
        }

        // 1. ����׼����ê������ (0-1��Χ) ת��Ϊ��Ļ��������
        //    (��ȻViewportToWorldPoint����ֱ����0-1���꣬������Ϊ������չʾ����)
        //    Vector3 screenPoint = new Vector3(Screen.width * screenAnchorPoint.x, Screen.height * screenAnchorPoint.y, distanceFromCamera);

        // 2. ��ֱ�ӵķ�ʽ��ʹ�� ViewportToWorldPoint ����
        //    �������ֱ��ʹ��0-1��Χ���ӿ����꣬���ʺϿ�ֱ��ʵĳ�����
        //    Vector3 viewportPoint = new Vector3(screenAnchorPoint.x, screenAnchorPoint.y, distanceFromCamera);

        // 3. �����Ŀ����������
        //    transform.position = mainCamera.ScreenToWorldPoint(screenPoint); // ��Ӧ����ķ���1
        transform.position = mainCamera.ViewportToWorldPoint(new Vector3(screenAnchorPoint.x, screenAnchorPoint.y, distanceFromCamera)); // ��Ӧ����ķ���2
    }
}