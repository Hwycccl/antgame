using UnityEngine;

public class DiagonalScroller : MonoBehaviour
{
    [Tooltip("滚动的速度和方向")]
    public Vector2 scrollSpeed = new Vector2(-2f, -2f);

    private Transform _transform;
    private Vector2 _startPosition;

    // 瓦片尺寸，需要手动设置或自动计算
    [Tooltip("单个瓦片的宽度")]
    public float tileWidth = 20f;
    [Tooltip("单个瓦片的高度")]
    public float tileHeight = 20f;

    // 整个网格由多少个瓦片组成 (例如 3x3)
    [Tooltip("网格的宽度（单位：瓦片数）")]
    public float gridWidth = 3f;
    [Tooltip("网格的高度（单位：瓦片数）")]
    public float gridHeight = 3f;

    private float _wrapLimitX;
    private float _wrapLimitY;

    void Start()
    {
        _transform = transform;
        _startPosition = _transform.position;

        // 计算需要“回绕”的边界
        // 当一个瓦片移动的距离超过半个网格总长时，就让它回到起点，再由其他瓦片来填充
        _wrapLimitX = (tileWidth * gridWidth) / 2f;
        _wrapLimitY = (tileHeight * gridHeight) / 2f;
    }

    void Update()
    {
        // 移动当前对象
        _transform.Translate(scrollSpeed * Time.deltaTime);

        // 获取当前位置与起始位置的偏移量
        Vector2 offset = (Vector2)_transform.position - _startPosition;

        // 检查水平方向是否超出边界
        if (Mathf.Abs(offset.x) > _wrapLimitX)
        {
            // 如果超出了，就把它移回起始位置的另一端
            // (offset.x > 0 ? -1 : 1) 判断是向右还是向左超出了
            float moveAmount = tileWidth * gridWidth * (offset.x > 0 ? -1 : 1);
            _transform.position += new Vector3(moveAmount, 0, 0);
            _startPosition.x = _transform.position.x; // 更新起始位置，避免抖动
        }

        // 检查垂直方向是否超出边界
        if (Mathf.Abs(offset.y) > _wrapLimitY)
        {
            // 同理，处理垂直方向
            float moveAmount = tileHeight * gridHeight * (offset.y > 0 ? -1 : 1);
            _transform.position += new Vector3(0, moveAmount, 0);
            _startPosition.y = _transform.position.y; // 更新起始位置
        }
    }
}