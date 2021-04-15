using System;
using deVoid.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RestartScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private Button _buttonContinue;
    [SerializeField] private Animator _screenAnimator;
    [SerializeField] private TextMeshProUGUI _text;

    private const float MoveToSide = 320;

    private static readonly int Open = Animator.StringToHash("Open");

    private void OnEnable()
    {
        Signals.Get<MustShowRestartSignal>().AddListener(TurnOn);
    }

    private void OnDisable()
    {
        Signals.Get<MustShowRestartSignal>().RemoveListener(TurnOn);
    }

    private void Awake()
    {
        _content.SetActive(false);
        _buttonContinue.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        GameManager.I.NexLevel();
    }

    private void TurnOn(bool freeRetry)
    {
        _text.text = freeRetry ? "You died<br>but this one is on us" : "You're lucky<br>you have bonus lives";
        _content.SetActive(true);
        _screenAnimator.SetTrigger(Open);
    }
}
