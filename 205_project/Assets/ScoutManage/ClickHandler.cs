
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    // ���ǰѾɵı����������µġ��ɾ���ScoutWindowController
    public ScoutWindowController scoutWindowController;

    public void OnPointerClick(PointerEventData eventData)
    {
        // ȷ����Unity�༭�������ú����������
        if (scoutWindowController != null)
        {
            Debug.Log("������򱻵�������ڿ��ش��ڡ�");
            // ���������½ű���ķ���
            scoutWindowController.ToggleSelectionWindow();
        }
        else
        {
            Debug.LogError("���� " + gameObject.name + " �����ClickHandler���������ScoutWindowController��");
        }
    }
}