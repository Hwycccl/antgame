// 文件名: MusicManager.cs
// 放置于: Assets/script/
using UnityEngine;

/// <summary>
/// 全局音乐管理器，负责在整个游戏过程中播放背景音乐。
/// 这是一个单例，确保在所有场景中只有一个实例。
/// </summary>
public class MusicManager : MonoBehaviour
{
    // 单例实例
    public static MusicManager Instance { get; private set; }

    [Header("音乐设置")]
    [Tooltip("将你的背景音乐文件拖到这里")]
    public AudioClip backgroundMusic;

    private AudioSource audioSource;

    private void Awake()
    {
        // --- 单例模式实现 ---
        // 如果已经存在一个实例，并且不是当前这个，就销毁当前这个，保证唯一性
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // --- 让音乐管理器在切换场景时不被销毁 ---
        DontDestroyOnLoad(gameObject);

        // --- 初始化音频组件 ---
        // 为这个物体添加一个AudioSource组件，用来播放声音
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic; // 设置要播放的音乐
        audioSource.loop = true;            // 设置为循环播放
        audioSource.playOnAwake = false;    // 禁止在唤醒时自动播放，我们手动控制
    }

    private void Start()
    {
        // 游戏一开始就播放音乐
        PlayMusic();
    }

    /// <summary>
    /// 公开的播放方法
    /// </summary>
    public void PlayMusic()
    {
        // 确保有音乐文件，且当前没有在播放
        if (audioSource != null && backgroundMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("背景音乐已开始播放: " + backgroundMusic.name);
        }
    }

    /// <summary>
    /// 公开的停止方法
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("背景音乐已停止。");
        }
    }
}
