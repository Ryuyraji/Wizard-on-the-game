using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public bool drawMode = true;
    bool temp;

    [SerializeField]
    GameObject LinePrefab;          // 생성할 라인 프리팹

    [SerializeField]
    GameObject LineContainer;       // 라인 프리팹 생성 위치

    [SerializeField]
    public Color lineColor = Color.white;  // 선 색상 (기본값: 흰색)

    [SerializeField]
    public float lineWidth = 0.1f;         // 선 굵기 (기본값: 0.1f)

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    // 마우스 포지션
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

            // 마우스 클릭 시 - 라인 인스턴스 생성
            if (Input.GetMouseButtonDown(0))
                LineStart();

            // 마우스 클릭 중
            else if (drawMode && Input.GetMouseButton(0))
                LineDraw();

            // 마우스 클릭 해제 시
            else if (Input.GetMouseButtonUp(0))
                LineEnd();
        }

        temp = drawMode;

       /* if (Input.GetMouseButtonDown(1)) // 우클릭으로 색 채우기 실행
        {
            Vector2 mousePos = Input.mousePosition;
            FillColorInsideArea(mousePos, Color.red); // 빨간색으로 채우기
        }*/
    }

    //Flood Fill이 경계를 넘어가지 않도록 EdgeCollider2D를 활용하여 경계를 감지
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
        // 라인 오브젝트 인스턴스 생성
        GameObject draw = Instantiate(LinePrefab);
        draw.transform.SetParent(LineContainer.transform);

        lineRenderer = draw.GetComponent<LineRenderer>();
        edgeCollider = draw.GetComponent<EdgeCollider2D>();

        ApplyLineMaterialColor(); // 색상 적용
        ApplyLineWidth();         // 선 굵기 적용

        // 마우스 위치 기준 라인 렌더러 포인트 설정
        Vector2 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.Add(startPoint);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);
    }

    void LineDraw()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 포인트가 충분히 다른 경우에만 추가
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
        ApplyLineMaterialColor(); // 색상 변경
    }

    public void ChangeLineWidth(float newWidth)
    {
        lineWidth = Mathf.Clamp(newWidth, 0.1f, 1f);  // 선 굵기 범위 제한
        ApplyLineWidth();  // 선 굵기 적용
    }

    // 머티리얼을 이용한 색상 적용
    void ApplyLineMaterialColor()
    {
        if (lineRenderer != null)
        {
            Material mat = lineRenderer.material;
            if (mat != null)
            {
                mat.color = lineColor;  // 머티리얼의 색상을 변경
            }
        }
    }

    // 선의 굵기를 적용하는 메서드
    void ApplyLineWidth()
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }

    //페인트 기능 구현 메서드
    /*
    void FillColorInsideArea(Vector2 screenPoint, Color fillColor)
    {
        // 카메라 확인
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera가 설정되어 있지 않습니다.");
            return;
        }

        // RenderTexture와 Texture2D 생성 및 확인
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture를 생성할 수 없습니다.");
            return;
        }

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        if (texture == null)
        {
            Debug.LogError("Texture2D를 생성할 수 없습니다.");
            return;
        }

        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        // RenderTexture에서 Texture2D로 읽기
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        Camera.main.targetTexture = null;
        RenderTexture.active = null;

        // Flood Fill 시작
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

        // UI에 Texture2D 적용 (RawImage가 null이 아닐 때만)
        var rawImage = GetComponent<UnityEngine.UI.RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = texture;
        }
        else
        {
            Debug.LogWarning("RawImage 컴포넌트가 없습니다. Texture2D가 화면에 적용되지 않았습니다.");
        }
    }*/


}