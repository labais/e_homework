using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AgentEffects : MonoBehaviour
{
    private static readonly int SliceAmount = Shader.PropertyToID("_SliceAmount");
    private const float DurationSec = 3;

    public void AnimateDeath(Action callback = null)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        var materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            materials.Add(renderer.material);
        }

        foreach (var material in materials)
        {
            DOVirtual.Float(0, 1, DurationSec, (percentage) =>
            {
                var eased = DOVirtual.EasedValue(0, 1, percentage, Ease.OutQuad);
                material.SetFloat(SliceAmount, eased);
            });
        }

        AsyncManager.I.Delay(TimeSpan.FromSeconds(DurationSec + .01f), () => { callback.TryInvoke(); });
    }
}