// �ļ���: EffectManager.cs (�Ѹ���)
using UnityEngine;

// ��� RequireComponent ���ԣ�ȷ��������������һ�� AudioSource ���
[RequireComponent(typeof(AudioSource))]
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("����Ч��Ԥ�Ƽ�")]
    [Tooltip("���ƺϳ�ʱ���ŵĻ�Ч��")]
    [SerializeField] private ParticleSystem combinationEffectPrefab;

    [Tooltip("��������ʱ���ŵ���ɢЧ��")]
    [SerializeField] private ParticleSystem dissipateEffectPrefab;

    // --- �������� #1: �����Ч�ֶ� ---
    [Header("��Ч����")]
    [Tooltip("���Ƴɹ��ϳ�ʱ���ŵ���Ч")]
    [SerializeField] private AudioClip combinationSound;
    // --- ����������� ---

    // ���� AudioSource ���
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

        // --- �������� #2: ��ȡ AudioSource ��� ---
        audioSource = GetComponent<AudioSource>();
        // --- ����������� ---
    }

    public void PlayCombinationEffect(Vector3 position)
    {
        if (combinationEffectPrefab != null)
        {
            ParticleSystem effectInstance = Instantiate(combinationEffectPrefab, position, Quaternion.identity);
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }

        // --- �������� #3: �ڲ����Ӿ���Ч��ͬʱ��������Ч ---
        if (combinationSound != null && audioSource != null)
        {
            // PlayOneShot �����ڲ���ϵ�ǰ�������ֵ�����²���һ������Ч
            audioSource.PlayOneShot(combinationSound);
        }
        // --- ����������� ---
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
