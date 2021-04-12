using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class FinishBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particles;

    void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(OnPlayerFinished);
        foreach (var particle in _particles)
        {
            particle.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(OnPlayerFinished);
    }

    private void OnPlayerFinished()
    {
        foreach (var particle in _particles)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }
    }
}