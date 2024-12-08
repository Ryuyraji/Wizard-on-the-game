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
    public bool eraseWholeObject = true;       // true : ��ü����, false : �κл���

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

        // ������ ���� ������Ʈ ��� ����
        foreach (Transform child in LineContainer.transform)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider is EdgeCollider2D && collider.gameObject == child.gameObject)
                {
					Destroy(child.gameObject);

					//if (eraseWholeObject)
					//{
					//    // ��ü ����
					//    Destroy(child.gameObject);
					//}
					//else
					//{
					//    // �κ� ����
					//    PartiallyEraseLine(collider);
					//}
					//break;

				}
            }
        }
    }

	// ������Ʈ ��ü�� �ƴ� ���� �κи� ��������
	void PartiallyEraseLine(Collider2D collider)
    {
        // ���� ���� ������Ʈ ��������
		LineRenderer lineRenderer = collider.GetComponent<LineRenderer>();
		EdgeCollider2D edgeCollider = collider.GetComponent<EdgeCollider2D>();

		if (lineRenderer != null && edgeCollider != null)
		{
			// LineRenderer�� �� ������ ��������
			// LineRenderer�� 3D ��ǥ�迡�� �����ϱ� ������ Vector3�� ����

			List<Vector3> points = new List<Vector3>();
			for (int i = 0; i < lineRenderer.positionCount; i++)
			{
				points.Add(lineRenderer.GetPosition(i));
			}

            // ���� ���� �ε��� ã��
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 mousePos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
			List<Vector3> newPoints = new List<Vector3>();
			foreach (var point in points)
			{
				// �ݰ� ���� ���� ����
				if (Vector3.Distance(point, mousePos) > radius)
				{
					newPoints.Add(point);
				}
			}

			// LineRenderer ������Ʈ
			lineRenderer.positionCount = newPoints.Count;
			lineRenderer.SetPositions(newPoints.ToArray());

			// EdgeCollider2D ������Ʈ
			List<Vector2> edgePoints = newPoints.ConvertAll(p => new Vector2(p.x, p.y));
			edgeCollider.points = edgePoints.ToArray();

			// ���� ���ٸ� ������Ʈ ����
			if (newPoints.Count == 0) Destroy(collider.gameObject);
		}
	}
}
