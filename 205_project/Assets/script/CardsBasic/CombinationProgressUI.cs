// 文件路径: Assets/script/CardsBasic/CombinationProgressUI.cs
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CombinationProgressUI : MonoBehaviour
{
    [Tooltip("用于填充进度条的UI Image组件")]
    [SerializeField]
    private Image progressBarFill;

    private Camera mainCamera;
    private bool isInitialized = false;

    void Awake()
    {
        // --- DEBUG ---
        Debug.Log($"[ProgressUI] 进度条 '{gameObject.name}' AWAKE。IsPlaying: {Application.isPlaying}");
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
            Debug.LogError("[ProgressUI] 严重错误: 场景中找不到主摄像机 (Main Camera)！");
            isInitialized = false;
            return;
        }
        Debug.Log("[ProgressUI] 成功找到主摄像机。");
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
            Debug.LogError($"[ProgressUI] 错误: '{gameObject.name}' 上的 progressBarFill 字段是空的，请在预制件里设置它！");
        }
    }
}