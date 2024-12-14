using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideOrShow : MonoBehaviour
{
    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIObject())
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    bool IsPointerOverUIObject()
    {
        // Ŭ�� ��ġ�� �������� Raycast ����
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Raycast ��� ����
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // ����� ������ UI ���� Ŭ������ ����
        return results.Count > 0;
    }
}
