using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public float offset = 25;
    internal Node end;
    internal Node start;

    public float disFromEnd;
    public LineRenderer lineRenderer;
    public NoodlePath noodlePath;
    public float zoom = 1;
    public float divMultiplier;

    public float LineWidth = 1;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.widthMultiplier = LineWidth;
    }

    // Update is called once per frame
    void Update()
    {
        var startPos = GetPos(start);
        var endPos = GetPos(end);

        if (startPos != endPos)
        {
            DrawNoodle(noodlePath, new List<Vector3>
            {
                startPos.position,
                endPos.position
            });
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private Transform GetPos(Node node)
    {
        if (node == null)
        {
            Debug.Log("");
        }

        return node.isVisible || node.deviceUI == null ? node.transform : node.deviceUI.transform;
    }

    private Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t, uu = u * u;
        float uuu = uu * u, ttt = tt * t;
        return new Vector2(
            (uuu * p0.x) + (3 * uu * t * p1.x) + (3 * u * tt * p2.x) + (ttt * p3.x),
            (uuu * p0.y) + (3 * uu * t * p1.y) + (3 * u * tt * p2.y) + (ttt * p3.y)
        );
    }

    private void DrawAAPolyLineNonAlloc(Vector3 vector3, Vector2 start_1)
    {
        throw new NotImplementedException();
    }

    /// <summary> Draw a bezier from output to input in grid coordinates </summary>
    public void DrawNoodle(NoodlePath path, List<Vector3> gridPoints)
    {

        int length = gridPoints.Count;
        switch (path)
        {
            case NoodlePath.Curvy:
                Vector2 outputTangent = Vector2.right;
                for (int i = 0; i < length - 1; i++)
                {
                    Vector2 inputTangent;
                    // Cached most variables that repeat themselves here to avoid so many indexer calls :p
                    Vector2 point_a = gridPoints[i];
                    Vector2 point_b = gridPoints[i + 1];
                    float dist_ab = Vector2.Distance(point_a, point_b);
                    if (i == 0) outputTangent = zoom * dist_ab * 0.01f * Vector2.right;
                    if (i < length - 2)
                    {
                        Vector2 point_c = gridPoints[i + 2];
                        Vector2 ab = (point_b - point_a).normalized;
                        Vector2 cb = (point_b - point_c).normalized;
                        Vector2 ac = (point_c - point_a).normalized;
                        Vector2 p = (ab + cb) * 0.5f;
                        float tangentLength = (dist_ab + Vector2.Distance(point_b, point_c)) * 0.005f * zoom;
                        float side = ((ac.x * (point_b.y - point_a.y)) - (ac.y * (point_b.x - point_a.x)));

                        p = tangentLength * Mathf.Sign(side) * new Vector2(-p.y, p.x);
                        inputTangent = p;
                    }
                    else
                    {
                        inputTangent = zoom * dist_ab * 0.01f * Vector2.left;
                    }

                    // Calculates the tangents for the bezier's curves.
                    float zoomCoef = 50 / zoom;
                    Vector2 tangent_a = point_a + outputTangent * zoomCoef;
                    Vector2 tangent_b = point_b + inputTangent * zoomCoef;
                    // Hover effect.
                    int division = Mathf.RoundToInt(divMultiplier * dist_ab) + 3;
                    // Coloring and bezier drawing.
                    Vector2 bezierPrevious = point_a;

                    lineRenderer.positionCount = division;
                    for (int j = 1; j <= division; ++j)
                    {
                        Vector2 bezierNext = CalculateBezierPoint(point_a, tangent_a, tangent_b, point_b, j / (float)division);
                        lineRenderer.SetPosition(j - 1, bezierPrevious);
                        bezierPrevious = bezierNext;
                    }
                    lineRenderer.SetPosition(division - 1, bezierPrevious);
                    outputTangent = -inputTangent;
                }
                break;
            case NoodlePath.Angled:
                for (int i = 0; i < length - 1; i++)
                {
                    if (i == length - 1) continue; // Skip last index
                    if (gridPoints[i].x <= gridPoints[i + 1].x - (50))
                    {
                        var midpoint = Vector3.Lerp(gridPoints[i], gridPoints[i + 1], 0.5f);
                        Vector2 start_1 = gridPoints[i];
                        Vector2 end_1 = gridPoints[i + 1];
                        start_1.x = midpoint.x;
                        end_1.x = midpoint.x;
                        lineRenderer.positionCount = 4;
                        lineRenderer.SetPositions(new Vector3[] { gridPoints[i], start_1, end_1, gridPoints[i + 1] });
                    }
                    else
                    {
                        var midpoint = Vector3.Lerp(gridPoints[i], gridPoints[i + 1], 0.5f);
                        Vector2 start_1 = gridPoints[i];
                        Vector2 end_1 = gridPoints[i + 1];
                        start_1.x += offset;
                        end_1.x -= offset;
                        Vector2 start_2 = start_1;
                        Vector2 end_2 = end_1;
                        start_2.y = midpoint.y;
                        end_2.y = midpoint.y;
                        lineRenderer.positionCount = 6;
                        lineRenderer.SetPositions(new Vector3[] { gridPoints[i], start_1, start_2, end_2, end_1, gridPoints[i + 1] });
                    }
                }
                break;
        }
    }

    public enum NoodlePath
    {
        Angled,
        Curvy
    }
}
