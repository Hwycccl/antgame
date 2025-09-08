// ������: MainMenuManager.cs
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

    [Tooltip("��Ҫ���ص�������ͼƬ")]
    public GameObject mainBackground;

    [Tooltip("��������ѡ���ҳ��")]
    public GameObject settingsPage;

    [Header("UI Components")]
    // --- �������� #1: ��Ӷ����������ֱ������ ---
    [Tooltip("����ҳ���е���������")]
    public Slider volumeSlider;
    // -------------------------------------------

    [Header("��Ч")]
    [Tooltip("��ť�����Ч")]
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // ��ʼ����Ƶ���
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ����UIҳ��ĳ�ʼ״̬
        if (startPage != null) startPage.SetActive(true);
        if (menuPage != null) menuPage.SetActive(false);
        if (settingsPage != null) settingsPage.SetActive(false);

        // ��������ť�Ŀ�����
        ContinueButtonCheck();

        // --- �������� #2: ��ʼ�������ֵ ---
        // ȷ����Ϸһ��ʼ���������ʾ��ȷ�ĵ�ǰ����
        if (volumeSlider != null)
        {
            // PlayerPrefs��������������ҵ��������ã�����������Ĭ������Ϊ�������1
            // AudioListener.volume ������Ϸ��ȫ������
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

        // --- �������� #2: ������ͬʱ���ر���ͼƬ ---
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

            // --- �������� #3: ÿ�δ�����ʱ����ˢ�»����ֵ ---
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
            // �ڷ���ǰ��������������һ����ϰ��
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