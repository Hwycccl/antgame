// 文件名: EffectManager.cs (已更新)
using UnityEngine;

// 添加 RequireComponent 特性，确保该物体上总有一个 AudioSource 组件
[RequireComponent(typeof(AudioSource))]
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("粒子效果预制件")]
    [Tooltip("卡牌合成时播放的火花效果")]
    [SerializeField] private ParticleSystem combinationEffectPrefab;

    [Tooltip("卡牌销毁时播放的消散效果")]
    [SerializeField] private ParticleSystem dissipateEffectPrefab;

    // --- 新增代码 #1: 添加音效字段 ---
    [Header("音效设置")]
    [Tooltip("卡牌成功合成时播放的音效")]
    [SerializeField] private AudioClip combinationSound;
    // --- 新增代码结束 ---

    // 引用 AudioSource 组件
    private AudioSource audioSource;

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

        // --- 新增代码 #2: 获取 AudioSource 组件 ---
        audioSource = GetComponent<AudioSource>();
        // --- 新增代码结束 ---
    }

    public void PlayCombinationEffect(Vector3 position)
    {
        if (combinationEffectPrefab != null)
        {
            ParticleSystem effectInstance = Instantiate(combinationEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }

        // --- 新增代码 #3: 在播放视觉特效的同时，播放音效 ---
        if (combinationSound != null && audioSource != null)
        {
            // PlayOneShot 可以在不打断当前背景音乐的情况下播放一次性音效
            audioSource.PlayOneShot(combinationSound);
        }
        // --- 新增代码结束 ---
    }

    public void PlayDissipateEffect(Vector3 position)
    {
        if (dissipateEffectPrefab != null)
        {
            ParticleSystem effectInstance = Instantiate(dissipateEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }
    }
}
