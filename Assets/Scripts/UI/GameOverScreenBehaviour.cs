using System;
using deVoid.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private Button _buttonRestart;
    [SerializeField] private Animator _screenAnimator;

    [SerializeField] private TextMeshProUGUI _textKills;
    [SerializeField] private TextMeshProUGUI _textLevel;
    [SerializeField] private TextMeshProUGUI _textHoles;
    [SerializeField] private TextMeshProUGUI _textTrail;

    private RectTransform _textKillsRt;
    private RectTransform _textLevelRt;
    private RectTransform _textHolesRt;
    private RectTransform _textTrailRt;
    private Vector3 _textKillsOrigPos;
    private Vector3 _textLevelsOrigPos;
    private Vector3 _textHolesOrigPos;
    private Vector3 _textTrailOrigPos;
    private const float MoveToSide = 320;
    private bool _on;

    Sequence _sequence;

    private static readonly int Open = Animator.StringToHash("Open");

    private void OnEnable()
    {
        Signals.Get<MustShowGameOverSignal>().AddListener(TurnOn);
    }

    private void OnDisable()
    {
        Signals.Get<MustShowGameOverSignal>().RemoveListener(TurnOn);
    }

    private void Awake()
    {
        _textKillsRt = (RectTransform) _textKills.transform;
        _textLevelRt = (RectTransform) _textLevel.transform;
        _textHolesRt = (RectTransform) _textHoles.transform;
        _textTrailRt = (RectTransform) _textTrail.transform;
        _textKillsOrigPos = _textKillsRt.anchoredPosition;
        _textLevelsOrigPos = _textLevelRt.anchoredPosition;
        _textHolesOrigPos = _textHolesRt.anchoredPosition;
        _textTrailOrigPos = _textTrailRt.anchoredPosition;

        _content.SetActive(false);
        _buttonRestart.onClick.AddListener(OnRestartClick);
    }

    private void OnRestartClick()
    {
        GameManager.I.RestartGame();
        SoundManager.I.Play("click", 1, true);
    }

    private void TurnOn()
    {
        if (_on) return;
        _on = true;
        
        _content.SetActive(true);
        _screenAnimator.SetTrigger(Open);

        // "for loops" ? never heard of those

        _textKillsRt.anchoredPosition3D = _textKillsOrigPos + Vector3.right * (MoveToSide);
        _textLevelRt.anchoredPosition3D = _textLevelsOrigPos + Vector3.right * (MoveToSide);
        _textHolesRt.anchoredPosition3D = _textHolesOrigPos + Vector3.right * (MoveToSide);
        _textTrailRt.anchoredPosition3D = _textTrailOrigPos + Vector3.right * (MoveToSide);
        _textKillsRt.gameObject.SetActive(false);
        _textLevelRt.gameObject.SetActive(false);
        _textHolesRt.gameObject.SetActive(false);
        _textTrailRt.gameObject.SetActive(false);
        _textKills.text = $"Total kills: 0";
        _textLevel.text = $"Max level: 0";
        _textHoles.text = $"Number of holes cut: 0";
        _textTrail.text = $"Total trail length: 0m";

        _sequence = DOTween.Sequence();
        _sequence.AppendInterval(1);

        _sequence.AppendCallback(() =>
        {
            _textKillsRt.gameObject.SetActive(true);
            DOVirtual.Float(1, 0, .7f, (percentage) =>
            {
                _textKillsRt.anchoredPosition3D = _textKillsOrigPos + Vector3.right * (MoveToSide * percentage);
            }).SetEase(Ease.OutBack);
        });
        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            DOVirtual.Float(0, 1, 1, (percentage) =>
            {
                var n = Mathf.RoundToInt(GameDataManager.I.Kills * percentage);
                _textKills.text = $"Total kills: {n}";
            }).SetEase(Ease.InSine);
        });

        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            _textLevelRt.gameObject.SetActive(true);
            DOVirtual.Float(1, 0, .7f, (percentage) =>
            {
                _textLevelRt.anchoredPosition3D = _textLevelsOrigPos + Vector3.right * (MoveToSide * percentage);
            }).SetEase(Ease.OutBack);
        });

        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            DOVirtual.Float(0, 1, 1, (percentage) =>
            {
                var n = Mathf.RoundToInt(GameDataManager.I.LevelNumber * percentage);
                _textLevel.text = $"Max level: {n}";
            }).SetEase(Ease.InSine);
        });

        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            _textHolesRt.gameObject.SetActive(true);
            DOVirtual.Float(1, 0, .7f, (percentage) =>
            {
                _textHolesRt.anchoredPosition3D = _textHolesOrigPos + Vector3.right * (MoveToSide * percentage);
            }).SetEase(Ease.OutBack);
        });

        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            DOVirtual.Float(0, 1, 1, (percentage) =>
            {
                var n = Mathf.RoundToInt(GameDataManager.I.NumberOfHoles * percentage);
                _textHoles.text = $"Number of holes cut: {n}";
            }).SetEase(Ease.InSine);
        });

        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            _textTrailRt.gameObject.SetActive(true);
            DOVirtual.Float(1, 0, .7f, (percentage) =>
            {
                _textTrailRt.anchoredPosition3D = _textTrailOrigPos + Vector3.right * (MoveToSide * percentage);
            }).SetEase(Ease.OutBack);
        });

        _sequence.AppendInterval(.5f);

        _sequence.AppendCallback(() =>
        {
            DOVirtual.Float(0, 1, 1, (percentage) =>
            {
                var n = (GameDataManager.I.TotalTrailLength * percentage);
                _textTrail.text = $"Total trail length: {n:0.0}m";
            }).SetEase(Ease.InSine);
        });
    }
}