using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MainMenu : MonoBehaviour
{
    // 按钮点击音效（可选）,导入点击音效文件到Assets,在MainMenuController的Inspector中：将音效文件拖到buttonClickSound字段
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // 初始化音频组件,指向当前脚本对象
        audioSource = gameObject.AddComponent<AudioSource>();

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
    void ContinueButtonCheck()//当前函数只能支持简单存档，且存档内容都不确定，功能待完善
    {
        GameObject continueButton = GameObject.Find("ContinueButton");
        if (continueButton != null)
        {
            continueButton.GetComponent<Button>().interactable =
                PlayerPrefs.HasKey("GameSaved");
        }
    }

    public void OnStartGame()
    {
        PlayButtonSound();

        // 删除旧存档（可选）
        PlayerPrefs.DeleteKey("GameSaved");

        // 加载游戏场景
        SceneManager.LoadScene("MainGameScene");
    }

    public void OnContinueGame()
    {
        PlayButtonSound();

        // 这里可以加载存档数据
        // 实际游戏数据加载应在游戏场景中进行

        SceneManager.LoadScene("MainGameScene");
    }
    //使用面板
    public void OnSettings()
    {
        PlayButtonSound();

        // 直接修改游戏设置（不使用面板）
        // 示例：切换音量开关
        //float currentVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //float newVolume = currentVolume > 0 ? 0 : 1;
        //PlayerPrefs.SetFloat("MasterVolume", newVolume);

        //Debug.Log("主音量设置为: " + newVolume);
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
    //以上功能十分基础
}