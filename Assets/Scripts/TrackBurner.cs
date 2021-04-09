using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TrackBurner : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    private List<Vector3> trailPoints = new List<Vector3>();
    private List<DateTime> trailTimes = new List<DateTime>();
    private Vector3 lastPoint;

    private readonly Vector3 DO = Vector3.up; // Debug Offset for vector drawing
    private readonly Vector3 DO2 = Vector3.up * 0.1f;

    private void Start()
    {
        AddPointAndCheckIfCrossed(transform.position);
    }

    void FixedUpdate()
    {
        if (lastPoint != (Vector3) transform.position)
        {
            lastPoint = transform.position;
            AddPointAndCheckIfCrossed(lastPoint);
            CutOffOldPoints();
        }
    }

    private void AddPointAndCheckIfCrossed(Vector3 newPoint)
    {
        trailPoints.Add(newPoint);
        trailTimes.Add(DateTime.Now);

        if (trailPoints.Count < 4) return;
        var penultimatePoint = trailPoints[trailPoints.Count - 2];

        for (var i = 0; i < trailPoints.Count - 2; i++)
        {
            if (AreLinesIntersecting(trailPoints[i], trailPoints[i + 1], penultimatePoint, newPoint))
            {
                //Do smtn w/ this area
                DrawArea(i);
                trailPoints.Clear();
                trailTimes.Clear();
                return;
            }
        }

        Debug.Log($"trail.Count={trailPoints.Count}");
    }

    private void DrawArea(int startIndex)
    {
        // @note -- it would be better to use intersection point not [i] and [last], but hopefully no one will notice 
        for (var i = startIndex; i < trailPoints.Count - 2; i++)
        {
            Debug.DrawLine(trailPoints[i] + DO2, trailPoints[i + 1] + DO2, Color.red, 5);
        }
    }

    // https://www.habrador.com/tutorials/math/5-line-line-intersection/
    private bool AreLinesIntersecting(Vector3 l1_p1, Vector3 l1_p2, Vector3 l2_p1, Vector3 l2_p2, bool shouldIncludeEndPoints = true)
    {
        l1_p1 = squashVector(l1_p1);
        l1_p2 = squashVector(l1_p2);
        l2_p1 = squashVector(l2_p1);
        l2_p2 = squashVector(l2_p2);

        //To avoid floating point precision issues we can add a small value
        float epsilon = 0.00001f;

        bool isIntersecting = false;

        float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

        //Make sure the denominator is > 0, if not the lines are parallel
        if (denominator != 0f)
        {
            float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
            float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

            //Are the line segments intersecting if the end points are the same
            if (shouldIncludeEndPoints)
            {
                //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
            else
            {
                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
        }

        return isIntersecting;
    }

    // From XZ plane uz XY plane
    Vector3 squashVector(Vector3 v3)
    {
        return new Vector3(v3.x, v3.z, 0);
    }

    private void CutOffOldPoints()
    {
        var cutoffIndex = -1;
        var cutoffTime = DateTime.Now - TimeSpan.FromSeconds(_trailRenderer.time);

        for (int i = 0; i < trailTimes.Count; i++)
        {
            if (trailTimes[i] > cutoffTime)
            {
                cutoffIndex = i;
                break;
            }
        }

        CutTailAtIndex(cutoffIndex);
    }

    private void CutTailAtIndex(int cutoffIndex)
    {
        // Remove form start of the list
        for (var i = 0; i < cutoffIndex; i++)
        {
            trailPoints.RemoveAt(0);
            trailTimes.RemoveAt(0);
        }

        Debug.Log($"CutTailAtIndex::num={cutoffIndex}");
    }
}