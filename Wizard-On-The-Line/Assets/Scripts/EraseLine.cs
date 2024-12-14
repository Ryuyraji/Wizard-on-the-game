using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EraseLine : MonoBehaviour
{
    public bool eraseMode = false;
    bool current;

    [SerializeField]
    public bool eraseWholeObject = true;       // true : 전체삭제, false : 부분삭제

    [SerializeField]
    GameObject LinePrefab;          // 생성할 라인 프리팹

    [SerializeField]
    GameObject LineContainer;

    [SerializeField]
    float radius = 0.1f;

    //Color lineColor = Color.white;
    //float lineWidth = 0.1f;

    void Start()
    {
        LineContainer = GameObject.Find("LineContainer");
        current = eraseMode;
    }

    void Update()
    {
        if (eraseMode)
        {
            //if (current != eraseMode)
            //    this.GetComponent<DrawLine>().OffSwitch();
            
            // 지우기 모드이면서 마우스 클릭 중일 때
            //if (Input.GetMouseButtonDown(0))
            //{
            //    LineErase();
            //}
			if (Input.GetMouseButton(0))
			{
				LineErase();
			}

		}
        current = eraseMode;
    }

    public void OnSwitch()
    {
        eraseMode = true;
        this.GetComponent<DrawLine>().OffSwitch();
    }

    public void OffSwitch()
    {
        eraseMode = false;
    }

    public void WholeOrPartialMode()
    {
        if (eraseWholeObject) eraseWholeObject = false;
        else eraseWholeObject = true;
    }

    void LineErase()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mouseWorldPos, radius);

        // 생성된 라인 오브젝트 대상 검증
        foreach (Transform child in LineContainer.transform)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider is EdgeCollider2D && collider.gameObject == child.gameObject)
                {
					//Destroy(child.gameObject);

					if (eraseWholeObject)
					{
						// 전체 삭제
						Destroy(child.gameObject);
					}
					else
					{
						// 부분 삭제
						PartialDestroy(collider);
					}
					break;

				}
            }
        }
    }

    // 오브젝트 전체가 아닌 닿은 부분만 지워지게
    void PartialDestroy(Collider2D collider)
    {
        // 라인 관련 컴포넌트 가져오기
        LineRenderer lineRenderer = collider.GetComponent<LineRenderer>();
        EdgeCollider2D edgeCollider = collider.GetComponent<EdgeCollider2D>();

        if (lineRenderer != null && edgeCollider != null)
        {
            // LineRenderer의 점 데이터 가져오기
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                points.Add(lineRenderer.GetPosition(i));
            }

            // 닿은 점의 인덱스 찾기
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mousePos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

            // 분리된 두 리스트 생성
            List<Vector3> firstHalf = new List<Vector3>();
            List<Vector3> secondHalf = new List<Vector3>();

            bool foundSplit = false;

            foreach (var point in points)
            {
                // 반경 내의 점을 기준으로 분리
                if (Vector3.Distance(point, mousePos) <= radius && !foundSplit)
                {
                    foundSplit = true;
                    continue;
                }

                if (!foundSplit)
                {
                    firstHalf.Add(point);
                }
                else
                {
                    secondHalf.Add(point);
                }
            }

            // 기존 LineRenderer 삭제
            Destroy(collider.gameObject);

            // 새로운 LineRenderer 생성
            if (firstHalf.Count >= 2)
            {
                CreateNewLineRenderer(firstHalf);
            }

            if (secondHalf.Count >= 2)
            {
                CreateNewLineRenderer(secondHalf);
            }
        }
    }

    void CreateNewLineRenderer(List<Vector3> points)
    {
        // LineRenderer Prefab 생성
        GameObject newLine = Instantiate(LinePrefab, LineContainer.transform);

        LineRenderer newLineRenderer = newLine.GetComponent<LineRenderer>();
        EdgeCollider2D newEdgeCollider = newLine.GetComponent<EdgeCollider2D>();
        newLineRenderer.positionCount = points.Count;
        newLineRenderer.SetPositions(points.ToArray());

        // EdgeCollider2D 업데이트
        List<Vector2> edgePoints = points.ConvertAll(p => new Vector2(p.x, p.y));
        newEdgeCollider.points = edgePoints.ToArray();

        newLineRenderer.startWidth = this.GetComponent<DrawLine>().lineWidth;
        newLineRenderer.endWidth = this.GetComponent<DrawLine>().lineWidth;

        Material mat = newLineRenderer.material;
        if (mat != null)
        {
            mat.color = this.GetComponent<DrawLine>().lineColor;
        }
    }
}
