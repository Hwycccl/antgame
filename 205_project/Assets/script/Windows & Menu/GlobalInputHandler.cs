using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GlobalInputHandler : MonoBehaviour
{
    void Update()
    {
        // ����������Ƿ񱻰���
        if (Input.GetMouseButtonDown(0))
        {
            // ���DescriptionManager�����ڣ�������������û��ʾ����ʲô������
            if (DescriptionManager.Instance == null || !DescriptionManager.Instance.descriptionBox.activeSelf)
            {
                return;
            }

            // ����һ��ָ���¼����ݣ������������߼��
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            // ����һ���б����洢�������߼�⵽�Ľ��
            List<RaycastResult> results = new List<RaycastResult>();

            // EventSystem��ͬʱ���UIԪ�غʹ���Physics Raycaster��2D/3D����
            EventSystem.current.RaycastAll(eventData, results);

            bool clickedOnCard = false;
            // �������м�⵽������
            foreach (RaycastResult result in results)
            {
                // ��鱻������������Ƿ���CardDragger�ű�
                // ����У���˵�����ǵ㵽��һ�ſ���
                if (result.gameObject.GetComponent<CardDragger>() != null)
                {
                    clickedOnCard = true;
                    break; // ��Ȼ�Ѿ�ȷ�ϵ㵽�����ˣ���û��Ҫ���������
                }
            }

            // ������������б���������壬��û�з����κ�һ���ǿ���
            if (!clickedOnCard)
            {
                // ��ô������������
                DescriptionManager.Instance.HideDescription();
            }
        }
    }
}
