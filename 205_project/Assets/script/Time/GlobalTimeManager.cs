using UnityEngine;

/// <summary>
/// 全局时间管理器
/// 负责记录游戏的总运行时长，并提供开始、暂停和继续的控制。
/// 这个时间是持续累加的，不会因昼夜更替而重置。
/// </summary>
public class GlobalTimeManager : MonoBehaviour
{
    // 使用单例模式，方便全局访问
    public static GlobalTimeManager Instance { get; private set; }

    /// <summary>
    /// 从计时开始以来，游戏流逝的总秒数
    /// </summary>
    public float TotalTimeElapsed { get; private set; }

    /// <summary>
    /// 时间是否处于暂停状态
    /// </summary>
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        // 标准的单例模式实现
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // （可选）如果希望切换场景时不销毁这个管理器，可以取消下面的注释
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 游戏开始时，默认是暂停的，等待外部指令开始
        IsPaused = true;
        TotalTimeElapsed = 0f;
    }

    private void Update()
    {
        // 如果时间没有暂停，就累加真实世界的时间增量
        if (!IsPaused)
        {
            TotalTimeElapsed += Time.deltaTime;
        }
    }

    // --- 公开给其他脚本调用的方法 ---

    /// <summary>
    /// 开始或继续计时
    /// </summary>
    public void StartOrResumeTimer()
    {
        IsPaused = false;
        Debug.Log("全局时间启动/继续。当前总时间: " + TotalTimeElapsed);
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    public void PauseTimer()
    {
        IsPaused = true;
        Debug.Log("全局时间已暂停。");
    }

    /// <summary>
    /// 完全重置时间
    /// </summary>
    public void ResetTimer()
    {
        IsPaused = true;
        TotalTimeElapsed = 0f;
        Debug.Log("全局时间已重置。");
    }
}