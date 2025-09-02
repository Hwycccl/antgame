using UnityEngine;

public class ParentScroller : MonoBehaviour
{
    [Tooltip("滚动的速度和方向")]
    public Vector2 scrollSpeed = new Vector2(-2f, -2f);

    [Tooltip("单个瓦片的宽度和高度")]
    public Vector2 tileSize = new Vector2(20f, 20f);

    private Vector3 _startPosition;

    void Start()
    {
        // 记录父对象的初始位置，通常是 (0, 0, 0)
        _startPosition = transform.position;
    }

    void Update()
    {
        // 1. 移动父对象，所有子对象（9个瓦片）会跟着移动
        transform.Translate(scrollSpeed * Time.deltaTime);

        // 2. 检查父对象是否已经移动了整整一个瓦片的距离
        // Mathf.Abs 取绝对值，所以无论哪个方向移动都有效
        if (Mathf.Abs(transform.position.x - _startPosition.x) >= tileSize.x)
        {
            // 3. 如果是，则将父对象沿X轴拉回一个瓦片的距离
            // scrollSpeed.x > 0 判断是向右还是向左移动
            float offset = (scrollSpeed.x > 0 ? -1 : 1) * tileSize.x;
            transform.position += new Vector3(offset, 0, 0);
        }

        if (Mathf.Abs(transform.position.y - _startPosition.y) >= tileSize.y)
        {
            // 4. 同理，处理Y轴
            float offset = (scrollSpeed.y > 0 ? -1 : 1) * tileSize.y;
            transform.position += new Vector3(0, offset, 0);
        }
    }
}