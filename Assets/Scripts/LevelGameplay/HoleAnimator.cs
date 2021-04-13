using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using DG.Tweening;
using UnityEngine;

public class HoleAnimator : MonoBehaviour
{
    private static readonly int SliceAmount = Shader.PropertyToID("_SliceAmount");

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
        const float beamDuration = .4f;
        const float shakeMagnitude = .3f;
        const float minD = 10;
        const float maxD = 20;
        const float minL = 2;
        const float maxL = 25;

        // Less shaking for holes farther from camera
        var distance = Vector3.Distance(Camera.main.transform.position, holeWall.position);
        var percentageDistance = Mathf.Clamp01((distance - minD) / (maxD - minD));
        var distanceFromCameraPenalty = (1 - percentageDistance) * 1.2f;

        var shapeLength = MyMaths.ShapeLength(hole.GetComponent<MeshFilter>().mesh.vertices);

        var lenghtBonus = 1 + (Mathf.Clamp01((shapeLength - minL) / (maxL - minL)) * .5f);

        // Debug.Log($"shapeLength={shapeLength} lenghtBonus={lenghtBonus} distance={distance} distanceFromCameraPenalty={distanceFromCameraPenalty}");

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            holeWall.gameObject.SetActive(true);
            Signals.Get<ShakeCameraSignal>().Dispatch(beamDuration, shakeMagnitude * distanceFromCameraPenalty * lenghtBonus);
        });
        sequence.AppendInterval(beamDuration);
        sequence.AppendCallback(() =>
        {
            holeWall.gameObject.SetActive(false);
            hole.gameObject.SetActive(true);
        });
        sequence.AppendCallback(() =>
        {
            var mat = hole.GetComponent<Renderer>().material;
            DOVirtual.Float(.05f, 1, 2.5f, (percentage) =>
            {
                var eased = DOVirtual.EasedValue(0, 1, percentage, Ease.OutQuad);
                mat.SetFloat(SliceAmount, eased);
            });
        });
    }
}