//ResourceDisplayUI.cs 
using UnityEngine;
using TMPro; // �����ʹ��TextMeshPro��������������������Text������ UnityEngine.UI

/// <summary>
/// ������UI����ʾ��ǰ��Դ�Ľű�
/// </summary>
public class ResourceDisplayUI : MonoBehaviour
{
    // ��Unity�༭���У������UI�ı�����ϵ�����
    [SerializeField] private TextMeshProUGUI fungusText;
    [SerializeField] private TextMeshProUGUI leafFragmentText;
    [SerializeField] private TextMeshProUGUI fecesText;

    void Update()
    {
        // ÿ֡����CardsManager��ȡ���µ���Դ����������UI
        if (CardsManager.Instance != null)
        {
            fungusText.text = $"���: {CardsManager.Instance.FungusAmount}";
            leafFragmentText.text = $"��Ҷ: {CardsManager.Instance.LeafFragmentAmount}";
            fecesText.text = $"���: {CardsManager.Instance.FecesAmount}";
        }
    }
}