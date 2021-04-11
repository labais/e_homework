using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class GameOverScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    private void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
    }

    private void Awake()
    {
        _content.SetActive(false);
    }

    private void OnPlayerDied()
    {
        _content.SetActive(true);
    }
}
