// ������: ElevationController.cs
// �����丽�ӵ������еĿ���Ԥ�Ƽ���
using UnityEngine;
using UnityEngine.UI;

public class ElevationController : MonoBehaviour
{
    private Canvas temporaryCanvas;
    private GraphicRaycaster temporaryRaycaster;
    private bool isElevated = false;

    // һ���������������ڿ�������״̬
    public void SetElevated(bool elevated)
    {
        if (isElevated == elevated) return; // ���״̬û�䣬��ʲô������

        isElevated = elevated;

        if (elevated)
        {
            // --- �����㼶 ---
            if (temporaryCanvas == null)
            {
                temporaryCanvas = gameObject.AddComponent<Canvas>();
                temporaryCanvas.overrideSorting = true;
                temporaryCanvas.sortingOrder = 30000; // ȷ�������

                temporaryRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        else
        {
            // --- �ָ��㼶 ---
            if (temporaryRaycaster != null) Destroy(temporaryRaycaster);
            if (temporaryCanvas != null) Destroy(temporaryCanvas);
        }
    }
}
