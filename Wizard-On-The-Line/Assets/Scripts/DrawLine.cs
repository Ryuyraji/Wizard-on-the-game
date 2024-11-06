using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    GameObject LinePrefab;          // 생성할 라인 프리팹

    [SerializeField]
    GameObject LineContainer;       // 라인 프리팹 생성 위치

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;

    // 마우스 포지션
    List<Vector2> mousePoint = new List<Vector2>();

    void Start()
    {
        LineContainer = GameObject.Find("LineContainer");
    }

    void Update()
    {
        // 마우스 클릭 시 - 라인 인스턴스 생성
        if (Input.GetMouseButtonDown(0))
            LineStart();

        // 마우스 클릭 중
        else if (Input.GetMouseButton(0))
            LineDraw();

        // 마우스 클릭 해제 시
        else if (Input.GetMouseButtonUp(0))
            LineEnd();
    }

    void LineStart()
    {
        // 라인 오브젝트 인스턴스 생성
        GameObject draw = Instantiate(LinePrefab);
        draw.transform.SetParent(LineContainer.transform);


        lineRenderer = draw.GetComponent<LineRenderer>();
        edgeCollider = draw.GetComponent<EdgeCollider2D>();

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
}
