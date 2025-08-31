using UnityEngine;
using UnityEngine.UI;

public class SettingUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingPanel;     // ���ô���
    public Button settingButton;        // ���ð�ť
    public Button closeButton;          // �رհ�ť
    public Slider volumeSlider;         // ��������

    void Start()
    {
        // ȷ�����ý���һ��ʼ�����ص�
        settingPanel.SetActive(false);

        // ��ť����¼���
        settingButton.onClick.AddListener(OpenSettingPanel);
        closeButton.onClick.AddListener(CloseSettingPanel);

        // ����������¼�
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
        // ����ȫ������
        AudioListener.volume = value;
    }
}

