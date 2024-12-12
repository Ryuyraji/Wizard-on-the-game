using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public bool drawMode = true;
    bool temp;

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
        temp = drawMode;
    }

    void Update()
    {

        if (drawMode)
        {
            if (temp != drawMode)
                this.GetComponent<EraseLine>().OffSwitch();

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

        temp = drawMode;

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

    //����Ʈ ��� ���� �޼���
    /*
    void FillColorInsideArea(Vector2 screenPoint, Color fillColor)
    {
        // ī�޶� Ȯ��
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera�� �����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        // RenderTexture�� Texture2D ���� �� Ȯ��
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture�� ������ �� �����ϴ�.");
            return;
        }

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        if (texture == null)
        {
            Debug.LogError("Texture2D�� ������ �� �����ϴ�.");
            return;
        }

        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        // RenderTexture���� Texture2D�� �б�
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        Camera.main.targetTexture = null;
        RenderTexture.active = null;

        // Flood Fill ����
        Vector2Int pixelPosition = new Vector2Int((int)screenPoint.x, (int)screenPoint.y);
        Color targetColor = texture.GetPixel(pixelPosition.x, pixelPosition.y);

        if (targetColor == fillColor) return;

        Queue<Vector2Int> pixels = new Queue<Vector2Int>();
        pixels.Enqueue(pixelPosition);

        while (pixels.Count > 0)
        {
            Vector2Int currentPixel = pixels.Dequeue();
            if (currentPixel.x < 0 || currentPixel.x >= texture.width || currentPixel.y < 0 || currentPixel.y >= texture.height)
                continue;

            if (texture.GetPixel(currentPixel.x, currentPixel.y) != targetColor)
                continue;

            texture.SetPixel(currentPixel.x, currentPixel.y, fillColor);

            pixels.Enqueue(new Vector2Int(currentPixel.x + 1, currentPixel.y));
            pixels.Enqueue(new Vector2Int(currentPixel.x - 1, currentPixel.y));
            pixels.Enqueue(new Vector2Int(currentPixel.x, currentPixel.y + 1));
            pixels.Enqueue(new Vector2Int(currentPixel.x, currentPixel.y - 1));
        }

        texture.Apply();

        // UI�� Texture2D ���� (RawImage�� null�� �ƴ� ����)
        var rawImage = GetComponent<UnityEngine.UI.RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = texture;
        }
        else
        {
            Debug.LogWarning("RawImage ������Ʈ�� �����ϴ�. Texture2D�� ȭ�鿡 ������� �ʾҽ��ϴ�.");
        }
    }*/


}