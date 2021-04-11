using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using DG.Tweening;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private static readonly int SliceAmount = Shader.PropertyToID("_SliceAmount");

    void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }
    void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
    }

    private void OnPlayerDied()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        var materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            materials.Add(renderer.material);
        }

        foreach (var material in materials)
        {
            DOVirtual.Float(0, 1, 3f, (percentage) =>
            {
                var eased = DOVirtual.EasedValue(0, 1, percentage, Ease.OutQuad);
                material.SetFloat(SliceAmount, eased);
            });
        }
       
    }
    
}