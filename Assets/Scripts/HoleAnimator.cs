using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using DG.Tweening;
using UnityEngine;

public class HoleAnimator : MonoBehaviour
{
   

    void OnEnable()
    {
        Signals.Get<HoleGeneratedSignal>().AddListener(OnHoleGenerated);
    }

    void OnDisable()
    {
        Signals.Get<HoleGeneratedSignal>().RemoveListener(OnHoleGenerated);
    }

    private void OnHoleGenerated(Transform hole, Transform holeWall)
    {
        Debug.Log("I know, right ?");

        holeWall.localScale = new Vector3(1, 50, 1);
        var beamDuration = .4f;
        var shakeMagnitude = .1f;

        var distance = Vector3.Distance(Camera.main.transform.position, holeWall.position);
        var minD = 10;
        var maxD = 20;
        var percentageDistance = Mathf.Clamp01((distance - minD)/ (maxD - minD));
        var distanceFromCameraPenalty = (1 - percentageDistance);
        Debug.Log($"d={distance} %={percentageDistance}");
    
    
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            holeWall.gameObject.SetActive(true);
            Signals.Get<ShakeCameraSignal>().Dispatch(beamDuration, shakeMagnitude * distanceFromCameraPenalty);
        });
        sequence.AppendInterval(beamDuration);
        sequence.AppendCallback(() => { holeWall.gameObject.SetActive(false); });
        sequence.AppendCallback(() => { hole.gameObject.SetActive(true); });
    }
}