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
        holeWall.localScale = new Vector3(1, 50, 1);
        var beamDuration = .4f;
        var shakeMagnitude = .1f;

        // Less shaking for holes farther from camera
        var distance = Vector3.Distance(Camera.main.transform.position, holeWall.position);
        const float minD = 10;
        const float maxD = 20;
        var percentageDistance = Mathf.Clamp01((distance - minD) / (maxD - minD));
        var distanceFromCameraPenalty = (1 - percentageDistance);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            holeWall.gameObject.SetActive(true);
            Signals.Get<ShakeCameraSignal>().Dispatch(beamDuration, shakeMagnitude * distanceFromCameraPenalty);
        });
        sequence.AppendInterval(beamDuration);
        sequence.AppendCallback(() =>
        {
            holeWall.gameObject.SetActive(false);
            hole.gameObject.SetActive(true);
        });
        sequence.AppendCallback(() =>
        {
            // StartCoroutine(Dissolve(hole));

            //DOVirtual.EasedValue(0, 1, t, Ease.InOutCirc);
            var mat = hole.GetComponent<Renderer>().material;
            DOVirtual.Float(.05f, 1, 2.5f, (percentage) =>
            {
                var eased = DOVirtual.EasedValue(0, 1, percentage, Ease.OutQuad);
                mat.SetFloat("_SliceAmount", eased);
            });
        });
    }
}