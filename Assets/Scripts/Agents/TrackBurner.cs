using System;
using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class TrackBurner : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRendererLong;
    [SerializeField] private TrailRenderer _trailRendererCutting;
    [SerializeField] DynamicGround _dynamicGround;

    private readonly List<Vector3> _trailPoints = new List<Vector3>();
    private readonly List<DateTime> _trailTimes = new List<DateTime>();
    private Vector3 _lastPoint;
    private bool _finished;

    private float _trailLengthSeconds = 50;
    private float _trailCuttingLengthSeconds = 20;

    private void Start()
    {
        AddPointAndCheckIfCrossed(transform.position);
        _trailRendererLong.time = _trailLengthSeconds;
        _trailRendererCutting.time = _trailCuttingLengthSeconds;
    }

    void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(OnPlayerFinished);
    }

    void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(OnPlayerFinished);
    }

    void FixedUpdate()
    {
        if (_finished) return;

        if (_lastPoint != transform.position)
        {
            _lastPoint = transform.position;
            AddPointAndCheckIfCrossed(_lastPoint);
            CutOffOldPoints();
        }

        CheckIfEnemyIsNotCuttingTrail();
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
                var shapePoints = _trailPoints.Skip(i).ToList();

                if (MyMaths.IsClockwise(shapePoints))
                {
                    shapePoints.Reverse();
                    // Can't have Clockwise shape, the walls then are generated inside out
                }

                _dynamicGround.UpdateGround(shapePoints);
                DrawArea(i);

                // Cut off all tail
                CutTailAtIndex(_trailPoints.Count - 1);
                _trailRendererCutting.Clear();

                return;
            }
        }

        // Debug.Log($"trail.Count={trailPoints.Count}");
    }

    private void DrawArea(int startIndex)
    {
        // @note -- it would be better to use intersection point not [i] and [last], but hopefully no one will notice 
        // for (var i = startIndex; i < _trailPoints.Count - 2; i++)
        // {
        //     Debug.DrawLine(_trailPoints[i] + DO2, _trailPoints[i + 1] + DO2, Color.red, 999);
        // }
    }

    private void CutOffOldPoints()
    {
        var cutoffIndex = -1;
        var cutoffTime = DateTime.Now - TimeSpan.FromSeconds(_trailCuttingLengthSeconds);

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

    private void OnPlayerFinished()
    {
        _finished = true;
    }

    private void CheckIfEnemyIsNotCuttingTrail()
    {
        const float EnemyDistanceToCut = .2f;
        if (EnemyManager.I.Enemies == null) return;
        foreach (var enemy in EnemyManager.I.Enemies)
        {
            for (int i = 0; i < _trailPoints.Count; i++)
            {
                var d = Vector3.Distance(_trailPoints[i], enemy.position);
                if (d < EnemyDistanceToCut)
                {
                    CutTailAtIndex(i);
                    _trailRendererCutting.Clear();
                        Debug.Log("Bums, AI tikko man nogrieza asti, vajag feijerverķus!");
                }
            }
        }
    }
}