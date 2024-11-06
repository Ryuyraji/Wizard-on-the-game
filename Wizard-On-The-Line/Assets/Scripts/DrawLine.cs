using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    GameObject LinePrefab;          // ������ ���� ������

    [SerializeField]
    GameObject LineContainer;       // ���� ������ ���� ��ġ

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    // ���콺 ������
    List<Vector2> mousePoint = new List<Vector2>();

    void Start()
    {
        LineContainer = GameObject.Find("LineContainer");
    }

    void Update()
    {
        // ���콺 Ŭ�� �� - ���� �ν��Ͻ� ����
        if (Input.GetMouseButtonDown(0))
            LineStart();

        // ���콺 Ŭ�� ��
        else if (Input.GetMouseButton(0))
            LineDraw();

        // ���콺 Ŭ�� ���� ��
        else if (Input.GetMouseButtonUp(0))
            LineEnd();
    }

    void LineStart()
    {
        // ���� ������Ʈ �ν��Ͻ� ����
        GameObject draw = Instantiate(LinePrefab);
        draw.transform.SetParent(LineContainer.transform);


        lineRenderer = draw.GetComponent<LineRenderer>();
        edgeCollider = draw.GetComponent<EdgeCollider2D>();

        // ���콺 ��ġ ���� ���� ������ ����Ʈ ����
        Vector2 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.Add(startPoint);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);
    }

    void LineDraw()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ����Ʈ�� ����� �ٸ� ��쿡�� �߰�
        if (mousePoint.Count == 0 || Vector2.Distance(mousePoint[mousePoint.Count - 1], pos) > 0.1f)
        {
            mousePoint.Add(pos);
            lineRenderer.positionCount = mousePoint.Count;
            lineRenderer.SetPosition(mousePoint.Count - 1, pos);
            edgeCollider.points = mousePoint.ToArray();
        }
    }

    void LineEnd()
    {
        mousePoint.Clear();
    }
}
