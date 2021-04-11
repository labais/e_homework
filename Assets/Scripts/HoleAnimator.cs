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
        var beamDuration = .4f;
        var shakeMagnitude = .1f;

        // Less shaking for holes farther from camera
        var distance = Vector3.Distance(Camera.main.transform.position, holeWall.position);
        const float minD = 10;
        const float maxD = 20;
        var percentageDistance = Mathf.Clamp01((distance - minD) / (maxD - minD));
        var distanceFromCameraPenalty = (1 - percentageDistance);

        var shapeLength = MyMaths.ShapeLength(hole.GetComponent<MeshFilter>().mesh.vertices);
     
        const float minL = 2;
        const float maxL = 25;
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