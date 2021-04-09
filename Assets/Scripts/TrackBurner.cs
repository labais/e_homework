using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TrackBurner : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] DynamicGround dynamicGround;
    
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
        if (lastPoint != transform.position)
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
            if (SomeMaths.AreLinesIntersecting(trailPoints[i], trailPoints[i + 1], penultimatePoint, newPoint))
            {
                dynamicGround.UpdateGround(trailPoints, i);
                DrawArea(i);
                CutTailAtIndex(i);
                return;
            }
        }

        // Debug.Log($"trail.Count={trailPoints.Count}");
    }

    private void DrawArea(int startIndex)
    {
        // @note -- it would be better to use intersection point not [i] and [last], but hopefully no one will notice 
        for (var i = startIndex; i < trailPoints.Count - 2; i++)
        {
            Debug.DrawLine(trailPoints[i] + DO2, trailPoints[i + 1] + DO2, Color.red, 5);
        }
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

        // Debug.Log($"CutTailAtIndex::num={cutoffIndex}");
    }
}