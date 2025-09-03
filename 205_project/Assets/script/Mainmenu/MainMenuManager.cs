using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ע�⣺�����������Ҫ���㴴�����ļ�����ȫһ��
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Pages")]
    [Tooltip("������ʼ��������Ϸ����ť��ҳ��")]
    public GameObject startPage;

    [Tooltip("�������˵���ť�����������õȣ���ҳ��")]
    public GameObject menuPage;

    // ��ť�����Ч
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // ��ʼ����Ƶ���
        audioSource = gameObject.AddComponent<AudioSource>();

        // ��Ϸ��ʼʱ��ֻ��ʾ��ʼҳ��
        if (startPage != null) startPage.SetActive(true);
        if (menuPage != null) menuPage.SetActive(false);

        // ����Ƿ��д浵����������ť�Ƿ����
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
            // ��menuPage�в���ContinueButton
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
    /// �ɳ�ʼ�ġ�������Ϸ����ť���ã������л������˵�ҳ��
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
        // ���ù��ܵ��߼�
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