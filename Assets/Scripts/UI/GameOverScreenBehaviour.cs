using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private Button _buttonRestart;
    [SerializeField] private Animator _screenAnimator;
    
    private static readonly int Open = Animator.StringToHash("Open");
    
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
        _buttonRestart.onClick.AddListener(OnRestartClick);
    }

    private void OnRestartClick()
    {
        GameManager.I.RestartGame();
    }

    private void OnPlayerDied()
    {
        _content.SetActive(true);
        _screenAnimator.SetTrigger(Open);
    }
}
