//ResourceDisplayUI.cs 
using UnityEngine;
using TMPro; // 如果你使用TextMeshPro，请用这个。如果用内置Text，请用 UnityEngine.UI

/// <summary>
/// 负责在UI上显示当前资源的脚本
/// </summary>
public class ResourceDisplayUI : MonoBehaviour
{
    // 在Unity编辑器中，将你的UI文本组件拖到这里
    [SerializeField] private TextMeshProUGUI fungusText;
    [SerializeField] private TextMeshProUGUI leafFragmentText;
    [SerializeField] private TextMeshProUGUI fecesText;

    void Update()
    {
        // 每帧都从CardsManager获取最新的资源数量并更新UI
        if (CardsManager.Instance != null)
        {
            fungusText.text = $"真菌: {CardsManager.Instance.FungusAmount}";
            leafFragmentText.text = $"碎叶: {CardsManager.Instance.LeafFragmentAmount}";
            fecesText.text = $"粪便: {CardsManager.Instance.FecesAmount}";
        }
    }
}