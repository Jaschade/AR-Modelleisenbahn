using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;

    List<Vector2> points;
    bool b_lineComplete = false;

    float lineDrawPrecision = 0.2f; // 0.3f 
    float lineMinDrawPrecision = 0.08f; // 0.15f
    float lineErasePrecision = 0.02f; // 0.01f

    public void UpdateLine(Vector2 mousePos)
    {        
        // initialize list of points
        if (points == null)
        {
            points = new List<Vector2>();
            SetPoint(mousePos);
        }

        // can't check erase yet
        if (points.Count == 1 && Vector2.Distance(points.Last(), mousePos) > lineMinDrawPrecision && Vector2.Distance(points.Last(), mousePos) < lineDrawPrecision)
        {
            SetPoint(mousePos);
        }

        // connect endpoints
        if (points.Count > 1 && Vector2.Distance(points.Last(), mousePos) > lineMinDrawPrecision && Vector2.Distance(points.Last(), mousePos) < lineDrawPrecision
                && Vector2.Distance(points.Last(), mousePos) < Vector2.Distance(points.ElementAt(points.Count - 2), mousePos)
                && Vector2.Distance(points.ElementAt(0), mousePos) < .1f)
        {
            ConnectPoints();
            b_lineComplete = true;
        }

        if (!b_lineComplete)
        {
            // draw line
            if (points.Count > 1 && Vector2.Distance(points.Last(), mousePos) > lineMinDrawPrecision && Vector2.Distance(points.Last(), mousePos) < lineDrawPrecision
                && Vector2.Distance(points.Last(), mousePos) < Vector2.Distance(points.ElementAt(points.Count - 2), mousePos))
            {
                SetPoint(mousePos);
            }

            // erase points in line
            if (points.Count > 1 && Vector2.Distance(points.Last(), mousePos) > lineErasePrecision && Vector2.Distance(points.Last(), mousePos) < lineDrawPrecision
                && Vector2.Distance(points.Last(), mousePos) > Vector2.Distance(points.ElementAt(points.Count - 2), mousePos))
            {
                RemovePoint(mousePos);
            }
        }
    }

    // add new point to line
    void SetPoint(Vector2 point)
    {
        points.Add(point);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);
    }

    // remove the latest point from line
    void RemovePoint(Vector2 point)
    {
        points.RemoveAt(points.Count - 1);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);
    }

    // complete line to a path
    void ConnectPoints()
    {
        // add first vector again to complete the path
        points.Add(points.ElementAt(0));

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, points.ElementAt(0));
    }

    // return the list of points
    public List<Vector2> GetLine()
    {
        return points;
    }
}