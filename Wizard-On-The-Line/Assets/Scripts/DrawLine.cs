using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public bool drawMode = true;
    bool current;

    [SerializeField]
    GameObject LinePrefab;          // ������ ���� ������

    [SerializeField]
    GameObject LineContainer;       // ���� ������ ���� ��ġ

    [SerializeField]
    public Color lineColor = Color.white;  // �� ���� (�⺻��: ���)

    [SerializeField]
    public float lineWidth = 0.1f;         // �� ���� (�⺻��: 0.1f)

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    // ���콺 ������
    List<Vector2> mousePoint = new List<Vector2>();

    void Start()
    {
        LineContainer = GameObject.Find("LineContainer");
        if (drawMode) this.GetComponent<EraseLine>().eraseMode = false;
        current = drawMode;
    }

    void Update()
    {

        if (drawMode)
        {
            //if (current != drawMode)
            //    this.GetComponent<EraseLine>().OffSwitch();

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

        current = drawMode;

       /* if (Input.GetMouseButtonDown(1)) // ��Ŭ������ �� ä��� ����
        {
            Vector2 mousePos = Input.mousePosition;
            FillColorInsideArea(mousePos, Color.red); // ���������� ä���
        }*/
    }

    //Flood Fill�� ��踦 �Ѿ�� �ʵ��� EdgeCollider2D�� Ȱ���Ͽ� ��踦 ����
  /*  bool IsInsideBoundary(Vector2 point)
    {
        return edgeCollider.OverlapPoint(point);
    }*/

    public void OnSwitch()
    {
        drawMode = true;
        this.GetComponent<EraseLine>().OffSwitch();
    }

    public void OffSwitch()
    {
        drawMode = false;
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