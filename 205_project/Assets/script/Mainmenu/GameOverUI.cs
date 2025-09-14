// 放置于: Assets/script/Windows & Menu/GameOverUI.cs (English Version)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The root object of the game over screen")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("The text field for displaying final stats")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Tooltip("The 'Restart' button")]
    [SerializeField] private Button restartButton;

    [Tooltip("The 'Quit' button")]
    [SerializeField] private Button quitButton;

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

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    /// <summary>
    /// Shows the game over screen and updates the stats text.
    /// </summary>
    /// <param name="daysSurvived">Total days the player survived</param>
    /// <param name="finalPopulation">Total ant population at the end</param>
    /// <param name="reason">The reason for the game over</param>
    public void ShowGameOverScreen(int daysSurvived, int finalPopulation, string reason)
    {
        if (gameOverPanel == null || statsText == null)
        {
            Debug.LogError("UI components for GameOverUI are not set in the Inspector!");
            return;
        }
        // --- 核心修改点：在这里停止音乐 ---
        // 在显示结束画面时，找到音乐管理器并调用停止方法
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
        }
        // --- 修改结束 ---
        // Format the text in English
        statsText.text = $"{reason}\n\n" +
                         $"You survived for: <color=yellow>{daysSurvived}</color> days\n" +
                         $"Your colony reached a size of: <color=yellow>{finalPopulation}</color> ants";

        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Reloads the current scene to restart the game.
    /// </summary>
    private void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    private void QuitGame()
    {
        Debug.Log("Player has chosen to quit the game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}