using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[ExecuteInEditMode]
public class InvertedCircleCollider : MonoBehaviour
{
    [SerializeField]
    private int numEdges = 60;
    [SerializeField]
    private float radius = 1f;

    void Start()
    {
        UpdateCollider(1f);
    }

    public void UpdateCollider(float edgeScale)
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[numEdges];
        float edgeRadius = radius;// + edgeCollider.edgeRadius;
        for (int i = 0; i < numEdges; ++i)
        {
            float angle = 2 * Mathf.PI * i / numEdges;
            float x = edgeRadius * Mathf.Cos(angle);
            float y = edgeRadius * Mathf.Sin(angle);
            points[i] = new Vector2(x, y);
        }

        edgeCollider.points = points;
    }
}
