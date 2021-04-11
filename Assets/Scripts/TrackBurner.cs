using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrackBurner : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;

    [FormerlySerializedAs("dynamicGround")] [SerializeField]
    DynamicGround _dynamicGround;

    private readonly List<Vector3> _trailPoints = new List<Vector3>();
    private readonly List<DateTime> _trailTimes = new List<DateTime>();
    private Vector3 _lastPoint;

    private readonly Vector3 DO = Vector3.up; // Debug Offset for vector drawing
    private readonly Vector3 DO2 = Vector3.up * 0.1f;

    private void Start()
    {
        AddPointAndCheckIfCrossed(transform.position);
    }

    void FixedUpdate()
    {
        if (_lastPoint != transform.position)
        {
            _lastPoint = transform.position;
            AddPointAndCheckIfCrossed(_lastPoint);
            CutOffOldPoints();
        }
    }

    private void AddPointAndCheckIfCrossed(Vector3 newPoint)
    {
        _trailPoints.Add(newPoint);
        _trailTimes.Add(DateTime.Now);
        
        if (_trailPoints.Count < 4) return;
        var penultimatePoint = _trailPoints[_trailPoints.Count - 2];

        for (var i = 0; i < _trailPoints.Count - 2; i++)
        {
            if (MyMaths.AreLinesIntersecting(_trailPoints[i], _trailPoints[i + 1], penultimatePoint, newPoint))
            {
                _dynamicGround.UpdateGround(_trailPoints, i);
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
        for (var i = startIndex; i < _trailPoints.Count - 2; i++)
        {
            Debug.DrawLine(_trailPoints[i] + DO2, _trailPoints[i + 1] + DO2, Color.red, 999);
        }
    }

    private void CutOffOldPoints()
    {
        var cutoffIndex = -1;
        var cutoffTime = DateTime.Now - TimeSpan.FromSeconds(_trailRenderer.time);

        for (int i = 0; i < _trailTimes.Count; i++)
        {
            if (_trailTimes[i] > cutoffTime)
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
            _trailPoints.RemoveAt(0);
            _trailTimes.RemoveAt(0);
        }

        // Debug.Log($"CutTailAtIndex::num={cutoffIndex}");
    }
}