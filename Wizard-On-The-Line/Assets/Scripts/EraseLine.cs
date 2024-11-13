using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseLine : MonoBehaviour
{
    public bool eraseMode = false;
    bool temp;

    [SerializeField]
    GameObject LineContainer;

    [SerializeField]
    float radius = 1.0f;

    void Start()
    {
        LineContainer = GameObject.Find("LineContainer");
        temp = eraseMode;
    }

    void Update()
    {
        if (eraseMode)
        {
            if (temp != eraseMode)
                this.GetComponent<DrawLine>().OffSwitch();
            
            // ����� ����̸鼭 ���콺 Ŭ�� ���� ��
            if (Input.GetMouseButtonDown(0))
            {
                LineErase();
            }
        }
        temp = eraseMode;
    }

    public void OffSwitch()
    {
        eraseMode = false;
    }

    void LineErase()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mouseWorldPos, radius);

        // ������ ���� ������Ʈ ��� ����
        foreach (Transform child in LineContainer.transform)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider is EdgeCollider2D && collider.gameObject == child.gameObject)
                {
                    // ���� ������Ʈ ����
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }
}
