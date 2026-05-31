using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class ParabolicGoal : MonoBehaviour
{
    public float width = 2.4f;
    public float height = 2.6f;
    public int segments = 24;
    public float lineWidth = 0.12f;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    void Awake()
    {
        BuildGoal();
    }

    void OnValidate()
    {
        BuildGoal();
    }

    void BuildGoal()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        if (lineRenderer == null || edgeCollider == null)
        {
            return;
        }

        segments = Mathf.Max(segments, 4);

        Vector3[] linePoints = new Vector3[segments + 1];
        Vector2[] colliderPoints = new Vector2[segments + 1];

        float halfWidth = width / 2f;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float x = Mathf.Lerp(-halfWidth, halfWidth, t);

            float normalizedX = x / halfWidth;
            float y = height * (1f - normalizedX * normalizedX);

            linePoints[i] = new Vector3(x, y, 0f);
            colliderPoints[i] = new Vector2(x, y);
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = linePoints.Length;
        lineRenderer.SetPositions(linePoints);

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.sortingOrder = 8;

        edgeCollider.points = colliderPoints;
        edgeCollider.edgeRadius = lineWidth;
    }
}