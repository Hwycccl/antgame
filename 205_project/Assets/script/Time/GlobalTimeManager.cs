using UnityEngine;

/// <summary>
/// ȫ��ʱ�������
/// �����¼��Ϸ��������ʱ�������ṩ��ʼ����ͣ�ͼ����Ŀ��ơ�
/// ���ʱ���ǳ����ۼӵģ���������ҹ��������á�
/// </summary>
public class GlobalTimeManager : MonoBehaviour
{
    // ʹ�õ���ģʽ������ȫ�ַ���
    public static GlobalTimeManager Instance { get; private set; }

    /// <summary>
    /// �Ӽ�ʱ��ʼ��������Ϸ���ŵ�������
    /// </summary>
    public float TotalTimeElapsed { get; private set; }

    /// <summary>
    /// ʱ���Ƿ�����ͣ״̬
    /// </summary>
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        // ��׼�ĵ���ģʽʵ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // ����ѡ�����ϣ���л�����ʱ���������������������ȡ�������ע��
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // ��Ϸ��ʼʱ��Ĭ������ͣ�ģ��ȴ��ⲿָ�ʼ
        IsPaused = true;
        TotalTimeElapsed = 0f;
    }

    private void Update()
    {
        // ���ʱ��û����ͣ�����ۼ���ʵ�����ʱ������
        if (!IsPaused)
        {
            TotalTimeElapsed += Time.deltaTime;
        }
    }

    // --- �����������ű����õķ��� ---

    /// <summary>
    /// ��ʼ�������ʱ
    /// </summary>
    public void StartOrResumeTimer()
    {
        IsPaused = false;
        Debug.Log("ȫ��ʱ������/��������ǰ��ʱ��: " + TotalTimeElapsed);
    }

    /// <summary>
    /// ��ͣ��ʱ
    /// </summary>
    public void PauseTimer()
    {
        IsPaused = true;
        Debug.Log("ȫ��ʱ������ͣ��");
    }

    /// <summary>
    /// ��ȫ����ʱ��
    /// </summary>
    public void ResetTimer()
    {
        IsPaused = true;
        TotalTimeElapsed = 0f;
        Debug.Log("ȫ��ʱ�������á�");
    }
}