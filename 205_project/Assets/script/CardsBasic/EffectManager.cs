// �ļ���: EffectManager.cs
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("����Ч��Ԥ�Ƽ�")]
    [Tooltip("���ƺϳ�ʱ���ŵĻ�Ч��")]
    [SerializeField] private ParticleSystem combinationEffectPrefab;

    [Tooltip("��������ʱ���ŵ���ɢЧ��")]
    [SerializeField] private ParticleSystem dissipateEffectPrefab;

    // ��Ҳ�������������һ����������Ż����ܣ���Ϊ�˼������������ʱʹ��Instantiate

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
    /// ��ָ��λ�ò��źϳ�Ч��
    /// </summary>
    /// <param name="position">����Ч����λ��</param>
    public void PlayCombinationEffect(Vector3 position)
    {
        if (combinationEffectPrefab != null)
        {
            Debug.Log("--- ���ڲ��źϳ�Ч����---");
            ParticleSystem effectInstance = Instantiate(combinationEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration); // ������Ϻ�����
        }
    }

    /// <summary>
    /// ��ָ��λ�ò�����ɢЧ��
    /// </summary>
    /// <param name="position">����Ч����λ��</param>
    public void PlayDissipateEffect(Vector3 position)
    {
        if (dissipateEffectPrefab != null)
        {
            Debug.Log("--- ���ڲ�����ɢЧ����---");
            ParticleSystem effectInstance = Instantiate(dissipateEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration); // ������Ϻ�����
        }
    }
}
