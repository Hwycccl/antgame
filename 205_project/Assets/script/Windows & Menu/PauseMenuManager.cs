// �ļ���: PauseMenuManager.cs
// ������: Assets/script/Windows & Menu/

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("UI����")]
    [Tooltip("�������ͣ�˵�UI Panel�ϵ�����")]
    public GameObject pauseMenuPanel;

    // ��̬���������������ű������Ϸ�Ƿ���ͣ
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
        // ��Ϸ��ʼʱȷ���˵��ǹرյ�
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // ���� Esc ���İ����¼�
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
    /// ��ͣ��Ϸ
    /// </summary>
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        // Time.timeScale = 0f �ᶳ����Ϸ�е���������Ͷ�����ʵ����������ͣ
        Time.timeScale = 0f;
        // ͬʱ����Ҳ��ͣ�Զ����ȫ�ּ�ʱ��
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.PauseTimer();
        }
        isPaused = true;
        Debug.Log("��Ϸ����ͣ��");
    }

    /// <summary>
    /// ������Ϸ
    /// </summary>
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        // Time.timeScale = 1f �ָ���������Ϸ�ٶ�
        Time.timeScale = 1f;
        // �ָ�ȫ�ּ�ʱ��
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.StartOrResumeTimer();
        }
        isPaused = false;
        Debug.Log("��Ϸ�Ѽ�����");
    }

    /// <summary>
    /// �����ô��� (���ܴ�ʵ��)
    /// </summary>
    public void OpenSettings()
    {
        // �����������Ӵ��������UI�����߼�
        Debug.Log("�����ô���...");
    }

    /// <summary>
    /// �浵 (���ܴ�ʵ��)
    /// </summary>
    public void SaveGame()
    {
        // ����һ���ǳ����ӵĹ��ܣ�����ֻ��һ���򵥵ı��
        // ʵ�ʵĴ浵ϵͳ��Ҫ�������п��Ƶ�λ�á�״̬�������Դ����Ϣ
        PlayerPrefs.SetInt("GameSaved", 1); // �����˵���"����"��ť����
        PlayerPrefs.Save();
        Debug.Log("��Ϸ�Ѵ浵��(ռλ��)");
    }

    /// <summary>
    /// ���ش浵 (���ܴ�ʵ��)
    /// </summary>
    public void LoadGame()
    {
        // ʵ�ʵļ���ϵͳ��Ҫ���ٵ�ǰ���ж���Ȼ����ݴ浵�ļ���������
        // ��򵥵ġ����ء��������¿�ʼ��ǰ����
        Debug.Log("���ش浵...(���¼��س���)");
        // �ڼ����³���ǰ������ָ�ʱ��߶ȣ�
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// �˳���Ϸ
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("�˳���Ϸ...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
