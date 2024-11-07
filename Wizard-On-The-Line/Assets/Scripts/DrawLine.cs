using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public bool drawMode = true;

    [SerializeField]
    GameObject LinePrefab;          // ������ ���� ������

    [SerializeField]
    GameObject LineContainer;       // ���� ������ ���� ��ġ

    [SerializeField]
    Color lineColor = Color.white;  // �� ���� (�⺻��: ���)

    [SerializeField]
    float lineWidth = 0.1f;         // �� ���� (�⺻��: 0.1f)

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    // ���콺 ������
    List<Vector2> mousePoint = new List<Vector2>();

    void Start()
    {
        LineContainer = GameObject.Find("LineContainer");
        if (drawMode) GetComponent<EraseLine>().eraseMode = false;
    }

    void Update()
    {
        if (drawMode)
        {
            GetComponent<EraseLine>().eraseMode = false;

            // ���콺 Ŭ�� �� - ���� �ν��Ͻ� ����
            if (Input.GetMouseButtonDown(0))
                LineStart();

            // ���콺 Ŭ�� ��
            else if (drawMode && Input.GetMouseButton(0))
                LineDraw();

            // ���콺 Ŭ�� ���� ��
            else if (Input.GetMouseButtonUp(0))
                LineEnd();
        }
    }

    void LineStart()
    {
        // ���� ������Ʈ �ν��Ͻ� ����
        GameObject draw = Instantiate(LinePrefab);
        draw.transform.SetParent(LineContainer.transform);

        lineRenderer = draw.GetComponent<LineRenderer>();
        edgeCollider = draw.GetComponent<EdgeCollider2D>();

        ApplyLineMaterialColor(); // ���� ����
        ApplyLineWidth();         // �� ���� ����

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

    public void ChangeLineColor(Color newColor)
    {
        lineColor = newColor;
        ApplyLineMaterialColor(); // ���� ����
    }

    public void ChangeLineWidth(float newWidth)
    {
        lineWidth = Mathf.Clamp(newWidth, 0.1f, 1f);  // �� ���� ���� ����
        ApplyLineWidth();  // �� ���� ����
    }

    // ��Ƽ������ �̿��� ���� ����
    void ApplyLineMaterialColor()
    {
        if (lineRenderer != null)
        {
            Material mat = lineRenderer.material;
            if (mat != null)
            {
                mat.color = lineColor;  // ��Ƽ������ ������ ����
            }
        }
    }

    // ���� ���⸦ �����ϴ� �޼���
    void ApplyLineWidth()
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }
}
