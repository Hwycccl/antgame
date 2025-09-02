using UnityEngine;
using UnityEngine.UI; // �������� UI �����ռ�
using TMPro;          // �������� TextMeshPro �����ռ�

/// <summary>
/// ��ҹ���ڼ�UI��ʾ������
/// ����� GlobalTimeManager ��ȡ��ʱ�䣬������ת��Ϊ������ÿ�ս���������UI��
/// </summary>
public class DayNightUIManager : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("һ����ҹ���ڵ�������")]
    [SerializeField] private float secondsPerDay = 60f; // ����������ÿ�������

    [Header("UI ����ο�")]
    [Tooltip("�뽫�����еĽ����� Image �ϵ�����")]
    [SerializeField] private Image progressBar;

    [Tooltip("�뽫�����е����� TextMeshPro �ϵ�����")]
    [SerializeField] private TMP_Text dayCountText;

    private int lastDay = -1; // ���ڼ�������Ƿ�仯�������ظ������ı�

    void Update()
    {
        // ȷ�� GlobalTimeManager ������ʱ��������
        if (GlobalTimeManager.Instance == null || GlobalTimeManager.Instance.IsPaused)
        {
            return; // ���ʱ����ͣ���򲻸���UI
        }

        // ����ʱ�ӻ�ȡ��ʱ��
        float totalTime = GlobalTimeManager.Instance.TotalTimeElapsed;

        // --- ���������ͽ��� ---
        // 1. ���㵱ǰ�ǵڼ��� (���� 125�� / 60�� = 2.08, ����ȡ��Ϊ2, �����ǵ� 2+1=3 ��)
        int currentDay = Mathf.FloorToInt(totalTime / secondsPerDay) + 1;

        // 2. ���㵱���Ѿ���ȥ�˶����� (���� 125�� % 60�� = 5��)
        float timeInCurrentDay = totalTime % secondsPerDay;

        // 3. ����������������� (���� 5�� / 60�� = 0.083)
        float fillPercentage = timeInCurrentDay / secondsPerDay;

        // --- ����UI ---
        if (progressBar != null)
        {
            progressBar.fillAmount = fillPercentage;
        }

        // �������������仯ʱ�Ÿ����ı�����������Ч
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