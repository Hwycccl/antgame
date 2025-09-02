using UnityEngine;

public class ParentScroller : MonoBehaviour
{
    [Tooltip("�������ٶȺͷ���")]
    public Vector2 scrollSpeed = new Vector2(-2f, -2f);

    [Tooltip("������Ƭ�Ŀ�Ⱥ͸߶�")]
    public Vector2 tileSize = new Vector2(20f, 20f);

    private Vector3 _startPosition;

    void Start()
    {
        // ��¼������ĳ�ʼλ�ã�ͨ���� (0, 0, 0)
        _startPosition = transform.position;
    }

    void Update()
    {
        // 1. �ƶ������������Ӷ���9����Ƭ��������ƶ�
        transform.Translate(scrollSpeed * Time.deltaTime);

        // 2. ��鸸�����Ƿ��Ѿ��ƶ�������һ����Ƭ�ľ���
        // Mathf.Abs ȡ����ֵ�����������ĸ������ƶ�����Ч
        if (Mathf.Abs(transform.position.x - _startPosition.x) >= tileSize.x)
        {
            // 3. ����ǣ��򽫸�������X������һ����Ƭ�ľ���
            // scrollSpeed.x > 0 �ж������һ��������ƶ�
            float offset = (scrollSpeed.x > 0 ? -1 : 1) * tileSize.x;
            transform.position += new Vector3(offset, 0, 0);
        }

        if (Mathf.Abs(transform.position.y - _startPosition.y) >= tileSize.y)
        {
            // 4. ͬ������Y��
            float offset = (scrollSpeed.y > 0 ? -1 : 1) * tileSize.y;
            transform.position += new Vector3(0, offset, 0);
        }
    }
}