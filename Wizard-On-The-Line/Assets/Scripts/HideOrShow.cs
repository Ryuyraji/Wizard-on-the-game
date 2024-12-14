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
        // 클릭 위치를 기준으로 Raycast 실행
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Raycast 결과 저장
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 결과가 있으면 UI 영역 클릭으로 간주
        return results.Count > 0;
    }
}
