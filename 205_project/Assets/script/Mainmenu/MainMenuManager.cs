using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    // 按钮点击音效
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // 初始化音频组件
        audioSource = gameObject.AddComponent<AudioSource>();

        // 游戏开始时，只显示初始页面
        if (startPage != null) startPage.SetActive(true);
        if (menuPage != null) menuPage.SetActive(false);

        // 检查是否有存档决定继续按钮是否可用
        ContinueButtonCheck();
    }

    void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    void ContinueButtonCheck()
    {
        if (menuPage != null)
        {
            // 在menuPage中查找ContinueButton
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

    /// <summary>
    /// 由初始的“进入游戏”按钮调用，用于切换到主菜单页面
    /// </summary>
    public void ShowMainMenu()
    {
        PlayButtonSound();
        if (startPage != null && menuPage != null)
        {
            startPage.SetActive(false);
            menuPage.SetActive(true);
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
        // 设置功能的逻辑
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
}