// 文件名: PauseMenuManager.cs
// 放置于: Assets/script/Windows & Menu/

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("UI设置")]
    [Tooltip("将你的暂停菜单UI Panel拖到这里")]
    public GameObject pauseMenuPanel;

    // 静态变量，方便其他脚本检查游戏是否暂停
    public static bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 游戏开始时确保菜单是关闭的
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // 监听 Esc 键的按下事件
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        // Time.timeScale = 0f 会冻结游戏中的所有物理和动画，实现真正的暂停
        Time.timeScale = 0f;
        // 同时我们也暂停自定义的全局计时器
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.PauseTimer();
        }
        isPaused = true;
        Debug.Log("游戏已暂停。");
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        // Time.timeScale = 1f 恢复正常的游戏速度
        Time.timeScale = 1f;
        // 恢复全局计时器
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.StartOrResumeTimer();
        }
        isPaused = false;
        Debug.Log("游戏已继续。");
    }

    /// <summary>
    /// 打开设置窗口 (功能待实现)
    /// </summary>
    public void OpenSettings()
    {
        // 在这里可以添加打开你的设置UI面板的逻辑
        Debug.Log("打开设置窗口...");
    }

    /// <summary>
    /// 存档 (功能待实现)
    /// </summary>
    public void SaveGame()
    {
        // 这是一个非常复杂的功能，这里只做一个简单的标记
        // 实际的存档系统需要保存所有卡牌的位置、状态，玩家资源等信息
        PlayerPrefs.SetInt("GameSaved", 1); // 让主菜单的"继续"按钮可用
        PlayerPrefs.Save();
        Debug.Log("游戏已存档！(占位符)");
    }

    /// <summary>
    /// 加载存档 (功能待实现)
    /// </summary>
    public void LoadGame()
    {
        // 实际的加载系统需要销毁当前所有对象，然后根据存档文件重新生成
        // 最简单的“加载”就是重新开始当前场景
        Debug.Log("加载存档...(重新加载场景)");
        // 在加载新场景前，必须恢复时间尺度！
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("退出游戏...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
