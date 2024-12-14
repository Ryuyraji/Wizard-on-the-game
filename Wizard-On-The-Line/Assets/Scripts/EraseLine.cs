using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EraseLine : MonoBehaviour
{
    public bool eraseMode = false;
    bool current;

    [SerializeField]
    public bool eraseWholeObject = true;       // true : ��ü����, false : �κл���

    [SerializeField]
    GameObject LinePrefab;          // ������ ���� ������

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

        // ������ ���� ������Ʈ ��� ����
        foreach (Transform child in LineContainer.transform)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider is EdgeCollider2D && collider.gameObject == child.gameObject)
                {
					//Destroy(child.gameObject);

					if (eraseWholeObject)
					{
						// ��ü ����
						Destroy(child.gameObject);
					}
					else
					{
						// �κ� ����
						PartialDestroy(collider);
					}
					break;

				}
            }
        }
    }

    // ������Ʈ ��ü�� �ƴ� ���� �κи� ��������
    void PartialDestroy(Collider2D collider)
    {
        // ���� ���� ������Ʈ ��������
        LineRenderer lineRenderer = collider.GetComponent<LineRenderer>();
        EdgeCollider2D edgeCollider = collider.GetComponent<EdgeCollider2D>();

        if (lineRenderer != null && edgeCollider != null)
        {
            // LineRenderer�� �� ������ ��������
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                points.Add(lineRenderer.GetPosition(i));
            }

            // ���� ���� �ε��� ã��
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mousePos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

            // �и��� �� ����Ʈ ����
            List<Vector3> firstHalf = new List<Vector3>();
            List<Vector3> secondHalf = new List<Vector3>();

            bool foundSplit = false;

            foreach (var point in points)
            {
                // �ݰ� ���� ���� �������� �и�
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

            // ���� LineRenderer ����
            Destroy(collider.gameObject);

            // ���ο� LineRenderer ����
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
        // LineRenderer Prefab ����
        GameObject newLine = Instantiate(LinePrefab, LineContainer.transform);

        LineRenderer newLineRenderer = newLine.GetComponent<LineRenderer>();
        EdgeCollider2D newEdgeCollider = newLine.GetComponent<EdgeCollider2D>();
        newLineRenderer.positionCount = points.Count;
        newLineRenderer.SetPositions(points.ToArray());

        // EdgeCollider2D ������Ʈ
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
