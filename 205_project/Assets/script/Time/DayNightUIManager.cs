using UnityEngine;
using UnityEngine.UI; // 必须引用 UI 命名空间
using TMPro;          // 必须引用 TextMeshPro 命名空间

/// <summary>
/// 昼夜周期及UI显示管理器
/// 负责从 GlobalTimeManager 获取总时间，并将其转换为天数和每日进度来更新UI。
/// </summary>
public class DayNightUIManager : MonoBehaviour
{
    [Header("周期设置")]
    [Tooltip("一个昼夜周期的总秒数")]
    [SerializeField] private float secondsPerDay = 60f; // 在这里设置每天的秒数

    [Header("UI 组件参考")]
    [Tooltip("请将场景中的进度条 Image 拖到这里")]
    [SerializeField] private Image progressBar;

    [Tooltip("请将场景中的天数 TextMeshPro 拖到这里")]
    [SerializeField] private TMP_Text dayCountText;

    private int lastDay = -1; // 用于检测天数是否变化，避免重复更新文本

    void Update()
    {
        // 确保 GlobalTimeManager 存在且时间在运行
        if (GlobalTimeManager.Instance == null || GlobalTimeManager.Instance.IsPaused)
        {
            return; // 如果时间暂停，则不更新UI
        }

        // 从主时钟获取总时间
        float totalTime = GlobalTimeManager.Instance.TotalTimeElapsed;

        // --- 计算天数和进度 ---
        // 1. 计算当前是第几天 (例如 125秒 / 60秒 = 2.08, 向下取整为2, 所以是第 2+1=3 天)
        int currentDay = Mathf.FloorToInt(totalTime / secondsPerDay) + 1;

        // 2. 计算当天已经过去了多少秒 (例如 125秒 % 60秒 = 5秒)
        float timeInCurrentDay = totalTime % secondsPerDay;

        // 3. 计算进度条的填充比例 (例如 5秒 / 60秒 = 0.083)
        float fillPercentage = timeInCurrentDay / secondsPerDay;

        // --- 更新UI ---
        if (progressBar != null)
        {
            progressBar.fillAmount = fillPercentage;
        }

        // 仅在天数发生变化时才更新文本，这样更高效
        if (currentDay != lastDay)
        {
            if (dayCountText != null)
            {
                dayCountText.text = "Day: " + currentDay;
            }
            lastDay = currentDay;
        }
    }
}