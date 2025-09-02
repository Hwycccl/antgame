using UnityEngine;

public class DiagonalScroller : MonoBehaviour
{
    [Tooltip("�������ٶȺͷ���")]
    public Vector2 scrollSpeed = new Vector2(-2f, -2f);

    private Transform _transform;
    private Vector2 _startPosition;

    // ��Ƭ�ߴ磬��Ҫ�ֶ����û��Զ�����
    [Tooltip("������Ƭ�Ŀ��")]
    public float tileWidth = 20f;
    [Tooltip("������Ƭ�ĸ߶�")]
    public float tileHeight = 20f;

    // ���������ɶ��ٸ���Ƭ��� (���� 3x3)
    [Tooltip("����Ŀ�ȣ���λ����Ƭ����")]
    public float gridWidth = 3f;
    [Tooltip("����ĸ߶ȣ���λ����Ƭ����")]
    public float gridHeight = 3f;

    private float _wrapLimitX;
    private float _wrapLimitY;

    void Start()
    {
        _transform = transform;
        _startPosition = _transform.position;

        // ������Ҫ�����ơ��ı߽�
        // ��һ����Ƭ�ƶ��ľ��볬����������ܳ�ʱ���������ص���㣬����������Ƭ�����
        _wrapLimitX = (tileWidth * gridWidth) / 2f;
        _wrapLimitY = (tileHeight * gridHeight) / 2f;
    }

    void Update()
    {
        // �ƶ���ǰ����
        _transform.Translate(scrollSpeed * Time.deltaTime);

        // ��ȡ��ǰλ������ʼλ�õ�ƫ����
        Vector2 offset = (Vector2)_transform.position - _startPosition;

        // ���ˮƽ�����Ƿ񳬳��߽�
        if (Mathf.Abs(offset.x) > _wrapLimitX)
        {
            // ��������ˣ��Ͱ����ƻ���ʼλ�õ���һ��
            // (offset.x > 0 ? -1 : 1) �ж������һ������󳬳���
            float moveAmount = tileWidth * gridWidth * (offset.x > 0 ? -1 : 1);
            _transform.position += new Vector3(moveAmount, 0, 0);
            _startPosition.x = _transform.position.x; // ������ʼλ�ã����ⶶ��
        }

        // ��鴹ֱ�����Ƿ񳬳��߽�
        if (Mathf.Abs(offset.y) > _wrapLimitY)
        {
            // ͬ������ֱ����
            float moveAmount = tileHeight * gridHeight * (offset.y > 0 ? -1 : 1);
            _transform.position += new Vector3(0, moveAmount, 0);
            _startPosition.y = _transform.position.y; // ������ʼλ��
        }
    }
}