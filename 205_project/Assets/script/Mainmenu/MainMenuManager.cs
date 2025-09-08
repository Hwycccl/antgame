// 放置于: MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 注意：这里的类名需要和你创建的文件名完全一致
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Pages")]
    [Tooltip("包含初始“进入游戏”按钮的页面")]
    public GameObject startPage;

    [Tooltip("包含主菜单按钮（继续、设置等）的页面")]
    public GameObject menuPage;

    [Tooltip("需要隐藏的主背景图片")]
    public GameObject mainBackground;

    [Tooltip("包含设置选项的页面")]
    public GameObject settingsPage;

    [Header("UI Components")]
    // --- 新增代码 #1: 添加对音量滑块的直接引用 ---
    [Tooltip("设置页面中的音量滑块")]
    public Slider volumeSlider;
    // -------------------------------------------

    [Header("音效")]
    [Tooltip("按钮点击音效")]
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // 初始化音频组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置UI页面的初始状态
        if (startPage != null) startPage.SetActive(true);
        if (menuPage != null) menuPage.SetActive(false);
        if (settingsPage != null) settingsPage.SetActive(false);

        // 检查继续按钮的可用性
        ContinueButtonCheck();

        // --- 新增代码 #2: 初始化滑块的值 ---
        // 确保游戏一开始，滑块就显示正确的当前音量
        if (volumeSlider != null)
        {
            // PlayerPrefs可以用来保存玩家的音量设置，这里我们先默认设置为最大音量1
            // AudioListener.volume 控制游戏的全局音量
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = AudioListener.volume;
        }
        // -----------------------------------
    }

    private void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void ContinueButtonCheck()
    {
        if (menuPage != null)
        {
            Transform continueButtonTransform = menuPage.transform.Find("ContinueButton");
            if (continueButtonTransform != null)
            {
                Button continueButton = continueButtonTransform.GetComponent<Button>();
                if (continueButton != null)
                {
                    continueButton.interactable = PlayerPrefs.HasKey("GameSaved");
                }
            }
        }
    }

    public void ShowMainMenu()
    {
        PlayButtonSound();
        if (startPage != null && menuPage != null)
        {
            startPage.SetActive(false);
            menuPage.SetActive(true);

        }

        // --- 新增代码 #2: 在这里同时隐藏背景图片 ---
        if (mainBackground != null)
        {
            mainBackground.SetActive(false);
        }
    }

    public void OnStartGame()
    {
        PlayButtonSound();
        PlayerPrefs.DeleteKey("GameSaved");
        SceneManager.LoadScene("MainGameScene");
    }

    public void OnContinueGame()
    {
        PlayButtonSound();
        SceneManager.LoadScene("MainGameScene");
    }

    public void OnSettings()
    {
        PlayButtonSound();
        if (menuPage != null && settingsPage != null)
        {
            menuPage.SetActive(false);
            settingsPage.SetActive(true);

            // --- 新增代码 #3: 每次打开设置时，都刷新滑块的值 ---
            if (volumeSlider != null)
            {
                volumeSlider.value = AudioListener.volume;
            }
            // ------------------------------------------------
        }
    }

    public void OnQuitGame()
    {
        PlayButtonSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnBackToMenu()
    {
        PlayButtonSound();
        if (menuPage != null && settingsPage != null)
        {
            // 在返回前保存音量设置是一个好习惯
            PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
            PlayerPrefs.Save();

            settingsPage.SetActive(false);
            menuPage.SetActive(true);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}