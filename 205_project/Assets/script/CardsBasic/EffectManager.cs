// 文件名: EffectManager.cs
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("粒子效果预制件")]
    [Tooltip("卡牌合成时播放的火花效果")]
    [SerializeField] private ParticleSystem combinationEffectPrefab;

    [Tooltip("卡牌销毁时播放的消散效果")]
    [SerializeField] private ParticleSystem dissipateEffectPrefab;

    // 你也可以在这里加入一个对象池来优化性能，但为了简单起见，我们暂时使用Instantiate

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

    /// <summary>
    /// 在指定位置播放合成效果
    /// </summary>
    /// <param name="position">播放效果的位置</param>
    public void PlayCombinationEffect(Vector3 position)
    {
        if (combinationEffectPrefab != null)
        {
            Debug.Log("--- 正在播放合成效果！---");
            ParticleSystem effectInstance = Instantiate(combinationEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration); // 播放完毕后销毁
        }
    }

    /// <summary>
    /// 在指定位置播放消散效果
    /// </summary>
    /// <param name="position">播放效果的位置</param>
    public void PlayDissipateEffect(Vector3 position)
    {
        if (dissipateEffectPrefab != null)
        {
            Debug.Log("--- 正在播放消散效果！---");
            ParticleSystem effectInstance = Instantiate(dissipateEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration); // 播放完毕后销毁
        }
    }
}
