// �ļ���: MusicManager.cs
// ������: Assets/script/
using UnityEngine;

/// <summary>
/// ȫ�����ֹ�������������������Ϸ�����в��ű������֡�
/// ����һ��������ȷ�������г�����ֻ��һ��ʵ����
/// </summary>
public class MusicManager : MonoBehaviour
{
    // ����ʵ��
    public static MusicManager Instance { get; private set; }

    [Header("��������")]
    [Tooltip("����ı��������ļ��ϵ�����")]
    public AudioClip backgroundMusic;

    private AudioSource audioSource;

    private void Awake()
    {
        // --- ����ģʽʵ�� ---
        // ����Ѿ�����һ��ʵ�������Ҳ��ǵ�ǰ����������ٵ�ǰ�������֤Ψһ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // --- �����ֹ��������л�����ʱ�������� ---
        DontDestroyOnLoad(gameObject);

        // --- ��ʼ����Ƶ��� ---
        // Ϊ����������һ��AudioSource�����������������
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic; // ����Ҫ���ŵ�����
        audioSource.loop = true;            // ����Ϊѭ������
        audioSource.playOnAwake = false;    // ��ֹ�ڻ���ʱ�Զ����ţ������ֶ�����
    }

    private void Start()
    {
        // ��Ϸһ��ʼ�Ͳ�������
        PlayMusic();
    }

    /// <summary>
    /// �����Ĳ��ŷ���
    /// </summary>
    public void PlayMusic()
    {
        // ȷ���������ļ����ҵ�ǰû���ڲ���
        if (audioSource != null && backgroundMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("���������ѿ�ʼ����: " + backgroundMusic.name);
        }
    }

    /// <summary>
    /// ������ֹͣ����
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("����������ֹͣ��");
        }
    }
}
