using UnityEngine;
using UnityEngine.UI;

public class SettingUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingPanel;     // 设置窗口
    public Button settingButton;        // 设置按钮
    public Button closeButton;          // 关闭按钮
    public Slider volumeSlider;         // 音量滑块

    void Start()
    {
        // 确保设置界面一开始是隐藏的
        settingPanel.SetActive(false);

        // 按钮点击事件绑定
        settingButton.onClick.AddListener(OpenSettingPanel);
        closeButton.onClick.AddListener(CloseSettingPanel);

        // 音量滑块绑定事件
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
    }

    void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    void AdjustVolume(float value)
    {
        // 调整全局音量
        AudioListener.volume = value;
    }
}

