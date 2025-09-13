// �ļ�·��: Assets/script/CardsBasic/CombinationProgressUI.cs
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CombinationProgressUI : MonoBehaviour
{
    [Tooltip("��������������UI Image���")]
    [SerializeField]
    private Image progressBarFill;

    private Camera mainCamera;
    private bool isInitialized = false;

    void Awake()
    {
        // --- DEBUG ---
        Debug.Log($"[ProgressUI] ������ '{gameObject.name}' AWAKE��IsPlaying: {Application.isPlaying}");
        if (!Application.isPlaying) return;
        InitializeCamera();
    }

    void OnEnable()
    {
        if (!Application.isPlaying) return;
        if (!isInitialized)
        {
            InitializeCamera();
        }
    }

    private void InitializeCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[ProgressUI] ���ش���: �������Ҳ���������� (Main Camera)��");
            isInitialized = false;
            return;
        }
        Debug.Log("[ProgressUI] �ɹ��ҵ����������");
        isInitialized = true;
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying || !isInitialized || mainCamera == null) return;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }

    public void UpdateProgress(float progress)
    {
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = Mathf.Clamp01(progress);
        }
        else
        {
            Debug.LogError($"[ProgressUI] ����: '{gameObject.name}' �ϵ� progressBarFill �ֶ��ǿյģ�����Ԥ�Ƽ�����������");
        }
    }
}