using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EraseLine : MonoBehaviour
{
    public bool eraseMode = false;
    bool temp;

    [SerializeField]
    GameObject LineContainer;

    [SerializeField]
    float radius = 0.1f;

    [SerializeField]
    public bool eraseWholeObject = true;       // true : 전체삭제, false : 부분삭제

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

        // 생성된 라인 오브젝트 대상 검증
        foreach (Transform child in LineContainer.transform)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider is EdgeCollider2D && collider.gameObject == child.gameObject)
                {
					Destroy(child.gameObject);

					//if (eraseWholeObject)
					//{
					//    // 전체 삭제
					//    Destroy(child.gameObject);
					//}
					//else
					//{
					//    // 부분 삭제
					//    PartiallyEraseLine(collider);
					//}
					//break;

				}
            }
        }
    }

	// 오브젝트 전체가 아닌 닿은 부분만 지워지게
	void PartiallyEraseLine(Collider2D collider)
    {
        // 라인 관련 컴포넌트 가져오기
		LineRenderer lineRenderer = collider.GetComponent<LineRenderer>();
		EdgeCollider2D edgeCollider = collider.GetComponent<EdgeCollider2D>();

		if (lineRenderer != null && edgeCollider != null)
		{
			// LineRenderer의 점 데이터 가져오기
			// LineRenderer는 3D 좌표계에서 동작하기 때문에 Vector3로 선언

			List<Vector3> points = new List<Vector3>();
			for (int i = 0; i < lineRenderer.positionCount; i++)
			{
				points.Add(lineRenderer.GetPosition(i));
			}

            // 닿은 점의 인덱스 찾기
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 mousePos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
			List<Vector3> newPoints = new List<Vector3>();
			foreach (var point in points)
			{
				// 반경 외의 점만 유지
				if (Vector3.Distance(point, mousePos) > radius)
				{
					newPoints.Add(point);
				}
			}

			// LineRenderer 업데이트
			lineRenderer.positionCount = newPoints.Count;
			lineRenderer.SetPositions(newPoints.ToArray());

			// EdgeCollider2D 업데이트
			List<Vector2> edgePoints = newPoints.ConvertAll(p => new Vector2(p.x, p.y));
			edgeCollider.points = edgePoints.ToArray();

			// 점이 없다면 오브젝트 삭제
			if (newPoints.Count == 0) Destroy(collider.gameObject);
		}
	}
}
