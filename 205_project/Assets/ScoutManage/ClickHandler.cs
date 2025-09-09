// �����: ScoutingZoneClickHandler.cs (�ռ����԰�)
using UnityEngine;
using UnityEngine.EventSystems; // ��Ҫ������������ռ�

public class ScoutingZoneClickHandler : MonoBehaviour
{
    // ��Unity�༭���У������ScoutingUIManager�ϵ�����
    public ScoutingUIManager uiManager;

    // OnMouseDown �����屻���ʱ����
    private void OnMouseDown()
    {
        // ·�� 1: ������¼��Ƿ񱻴���
        Debug.Log("--- OnMouseDown in ScoutingZoneClickHandler was called! ---");

        // ������飺������ָ������ͣ���κ�UIԪ���ϣ��Ͳ�ִ�к����߼�
        // ����һ��˫�ر��գ���ֹ��UI�Ϸ��ĵ����͸����Ϸ����
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.LogWarning("Click was blocked because the pointer is over a UI element.");
            return;
        }

        // ·�� 2: ��� uiManager �Ƿ���ȷ��ֵ
        if (uiManager != null)
        {
            Debug.Log("uiManager is NOT null. Attempting to call ToggleScoutingWindow...");
            // �������屻�����ʱ������UI���������򿪴���
            uiManager.ToggleScoutingWindow();
        }
        else
        {
            // ��� uiManager �ǿյģ������һ���ؼ�����
            Debug.LogError("CRITICAL ERROR: The 'uiManager' variable in ScoutingZoneClickHandler is NULL (empty). Please drag the UI Manager object into its slot in the Inspector!");
        }
    }
}