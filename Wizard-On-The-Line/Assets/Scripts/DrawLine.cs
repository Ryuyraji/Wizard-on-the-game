using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public bool drawMode = true;
    bool current;

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
        current = drawMode;
    }

    void Update()
    {

        if (drawMode)
        {
            //if (current != drawMode)
            //    this.GetComponent<EraseLine>().OffSwitch();

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

        current = drawMode;

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

   


}