using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MainMenu : MonoBehaviour
{
    // ��ť�����Ч����ѡ��,��������Ч�ļ���Assets,��MainMenuController��Inspector�У�����Ч�ļ��ϵ�buttonClickSound�ֶ�
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // ��ʼ����Ƶ���,ָ��ǰ�ű�����
        audioSource = gameObject.AddComponent<AudioSource>();

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
    void ContinueButtonCheck()//��ǰ����ֻ��֧�ּ򵥴浵���Ҵ浵���ݶ���ȷ�������ܴ�����
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

        // ɾ���ɴ浵����ѡ��
        PlayerPrefs.DeleteKey("GameSaved");

        // ������Ϸ����
        SceneManager.LoadScene("MainGameScene");
    }

    public void OnContinueGame()
    {
        PlayButtonSound();

        // ������Լ��ش浵����
        // ʵ����Ϸ���ݼ���Ӧ����Ϸ�����н���

        SceneManager.LoadScene("MainGameScene");
    }
    //ʹ�����
    public void OnSettings()
    {
        PlayButtonSound();

        // ֱ���޸���Ϸ���ã���ʹ����壩
        // ʾ�����л���������
        //float currentVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //float newVolume = currentVolume > 0 ? 0 : 1;
        //PlayerPrefs.SetFloat("MasterVolume", newVolume);

        //Debug.Log("����������Ϊ: " + newVolume);
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
    //���Ϲ���ʮ�ֻ���
}