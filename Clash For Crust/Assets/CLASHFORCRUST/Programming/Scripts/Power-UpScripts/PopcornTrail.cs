using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornTrail : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public const int maxPoints = 10; // point number on the trail (length)
    private Vector3[] positions;

    private void Start()
    {
        positions = new Vector3[maxPoints];

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.1f;
    }

    private void Update()
    {
        AddPoint(transform.position);

        lineRenderer.SetPositions(positions);
    }

    private void AddPoint(Vector3 newPosition)
    {
        for (int i = maxPoints - 1; i > 0; i--)
        {
            positions[i] = positions[i - 1];
        }

        positions[0] = newPosition;

        int pointCount = Mathf.Min(lineRenderer.positionCount + 1, maxPoints);
        lineRenderer.positionCount = pointCount;
    }
}