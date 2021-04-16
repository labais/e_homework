using System;
using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using DG.Tweening;
using UnityEngine;

public class TrackBurner : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRendererLong;
    [SerializeField] private TrailRenderer _trailRendererCutting;
    [SerializeField] DynamicGround _dynamicGround;
    [SerializeField] Material _trailCuttingMatA;
    [SerializeField] Material _trailCuttingMatB;

    private readonly List<Vector3> _trailPoints = new List<Vector3>();
    private readonly List<DateTime> _trailTimes = new List<DateTime>();
    private Vector3 _lastPoint;
    private bool _finished;

    private float _nominalCuttingTailStartingWidth;
    private float _trailLengthSeconds = 50;
    private float _trailCuttingLengthSecondsBase = 1.5f;
    private float _trailCuttingLengthSeconds;

    const float EnemyDistanceToCut = .2f;

    private void Start()
    {
        // quick fix: just copy the calculations
        var maxSpeed = PlayerController.MaxSpeedBase + (GameDataManager.I.GetUpgrade(UpgradeType.Speed) * .6f);
        var playerSpeedUpgradeRatio = maxSpeed / PlayerController.MaxSpeedBase;
        
        AddPointAndCheckIfCrossed(transform.position);
        _trailRendererLong.time = _trailLengthSeconds;
        _trailCuttingLengthSeconds = _trailCuttingLengthSecondsBase + (GameDataManager.I.GetUpgrade(UpgradeType.TrailLength) * .08f);
        _trailCuttingLengthSeconds /= playerSpeedUpgradeRatio; // faster player gets shorter trail duration resulting in the same trail spatial length  
        _trailRendererCutting.time = _trailCuttingLengthSeconds;
        _nominalCuttingTailStartingWidth = _trailRendererCutting.startWidth;
        _trailRendererCutting.material = _trailCuttingMatA;

        Debug.Log($"Trail len={_trailRendererCutting.time} sec lvl={GameDataManager.I.GetUpgrade(UpgradeType.TrailLength)}");
    }

    void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(OnPlayerFinished);
        Signals.Get<PlayerGotHitSignal>().AddListener(OnPlayerGotHit);
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }

    void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(OnPlayerFinished);
        Signals.Get<PlayerGotHitSignal>().RemoveListener(OnPlayerGotHit);
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
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

        CheckIfEnemyIsCuttingTrail();
    }

    private void AddPointAndCheckIfCrossed(Vector3 newPoint)
    {
        _trailPoints.Add(newPoint);
        _trailTimes.Add(DateTime.Now);

        if (_trailPoints.Count < 4) return;
        var penultimatePoint = _trailPoints[_trailPoints.Count - 2];

        GameDataManager.I.TotalTrailLength += (newPoint - penultimatePoint).magnitude / 100f;

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
                SoundManager.I.Play("vaporize", 1f, true);
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

    private void OnPlayerGotHit()
    {
        if (GameDataManager.I.GetUpgrade(UpgradeType.ImmunityBalls) > 0) return;

        CutTailAtIndex(_trailPoints.Count - 1);
        AnimateTailLost();
    }

    private void OnPlayerDied()
    {
        AnimateTailLost();
    }

    private void CheckIfEnemyIsCuttingTrail()
    {
        if (GameDataManager.I.GetUpgrade(UpgradeType.ImmunityTrailCut) > 0) return;
        if (EnemyManager.I.Enemies == null) return;

        foreach (var enemy in EnemyManager.I.Enemies)
        {
            if (enemy == null) continue;

            for (int i = 0; i < _trailPoints.Count; i++)
            {
                var d = Vector3.Distance(_trailPoints[i], enemy.transform.position);
                if (d < EnemyDistanceToCut)
                {
                    if (_trailPoints.Count > 20)
                    {
                        SoundManager.I.Play("trailcut", 1f, true);
                    }
                    CutTailAtIndex(_trailPoints.Count - 1);
                    AnimateTailLost();
                   
                }
            }
        }
    }

    private void AnimateTailLost()
    {
        var t = .05f;
        _trailRendererCutting.material = _trailCuttingMatB;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
            {
                DOVirtual.Float(.5f, 1, t, (percentage) =>
                {
                    _trailRendererCutting.startWidth = _nominalCuttingTailStartingWidth + (.4f * percentage);
                }).SetEase(Ease.InCirc);
            })
            .AppendInterval(t + .01f)
            .AppendCallback(() =>
            {
                DOVirtual.Float(1, .5f, t, (percentage) =>
                {
                    _trailRendererCutting.startWidth = _nominalCuttingTailStartingWidth + (.4f * percentage);
                }).SetEase(Ease.InCirc);
            })
            .AppendInterval(t + .01f)
            .AppendCallback(() =>
            {
                _trailRendererCutting.startWidth = _nominalCuttingTailStartingWidth;
                _trailRendererCutting.Clear();
                _trailRendererCutting.material = _trailCuttingMatA;
            })
            .AppendInterval(t + .01f)
            .AppendCallback(() =>
            {
                // better to be safe
                _trailRendererCutting.startWidth = _nominalCuttingTailStartingWidth;
                _trailRendererCutting.Clear();
                _trailRendererCutting.material = _trailCuttingMatA;
            });
    }
}