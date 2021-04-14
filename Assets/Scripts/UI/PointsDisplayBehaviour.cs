using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsDisplayBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _floatingText;

    private int _lastKnownPoints = 0;
    private int _lastKnownKills = 0;
    private RectTransform _floatingTextRt;

    private static readonly int StopTrigger = Animator.StringToHash("Stop");
    private static readonly int StartTrigger = Animator.StringToHash("Start");
    Sequence _sequence;

    private void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(Hide);
        Signals.Get<PlayerDiedSignal>().AddListener(Hide);
        Signals.Get<PointsChangedSignal>().AddListener(OnPointsChanged);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(Hide);
        Signals.Get<PlayerDiedSignal>().RemoveListener(Hide);
        Signals.Get<PointsChangedSignal>().RemoveListener(OnPointsChanged);
    }

    private void Start()
    {
        _floatingTextRt = (RectTransform)_floatingText.transform;
        _floatingText.gameObject.SetActive(false);

        _lastKnownPoints = GameDataManager.I.Points;
        _text.text = _lastKnownPoints.ToString();
    }

    private void OnPointsChanged()
    {
        _sequence?.Kill();

        var deltaPoints = GameDataManager.I.Points - _lastKnownPoints;
        var deltaKills = GameDataManager.I.Kills - _lastKnownKills;

        var visiblePoints = _lastKnownPoints;
        _lastKnownPoints = GameDataManager.I.Points;
        _lastKnownKills = GameDataManager.I.Kills;

        _floatingText.text = $"+{deltaPoints}";
        _floatingText.fontSize = 40 + deltaKills * 4;

        _floatingText.gameObject.SetActive(true);
        _floatingTextRt.localScale = Vector3.zero;
        _floatingText.color = Color.white;

        _sequence = DOTween.Sequence();

        _sequence.AppendCallback(() => { DOVirtual.Float(0, 1, .2f, (percentage) => { _floatingTextRt.localScale = Vector3.one * percentage; }).SetEase(Ease.OutCubic); });
        _sequence.AppendInterval(1);
        _sequence.AppendCallback(() =>
        {
            DOVirtual.Float(1, 0, 1f, (percentage) => { _floatingTextRt.localScale = Vector3.one * percentage; }).SetEase(Ease.InQuad);

            DOVirtual.Float(1, 0, 1, (percentage) =>
            {
                int p = Mathf.RoundToInt(deltaPoints * percentage);
                _floatingText.text = $"+{p}";
                _text.text = $"{(visiblePoints + deltaPoints - p)}";
            }).SetEase(Ease.InSine);
        });

        _sequence.AppendInterval(1.1f);
        _sequence.AppendCallback(() =>
        {
            _floatingText.gameObject.SetActive(false);
            _text.text = _lastKnownPoints.ToString();
        });
    }

    private void Hide()
    {
        _sequence?.Kill();
        _floatingText.gameObject.SetActive(false);
    }
}