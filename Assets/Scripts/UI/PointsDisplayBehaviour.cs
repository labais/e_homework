using deVoid.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PointsDisplayBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _floatingText;
    [SerializeField] private TextMeshProUGUI _floatingText2;

    private int _lastKnownPoints = 0;
    private int _lastKnownKills = 0;
    private RectTransform _floatingTextRt;
    private RectTransform _floatingText2Rt;
    private Vector3 _floatingTextOriginalPos;

    Sequence _sequence;

    private void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(Hide);
        Signals.Get<PlayerDiedSignal>().AddListener(Hide);
        Signals.Get<PointsChangedSignal>().AddListener(OnPointsChanged);
        Signals.Get<ShakeCoinsSignal>().AddListener(ShakeCoins);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(Hide);
        Signals.Get<PlayerDiedSignal>().RemoveListener(Hide);
        Signals.Get<PointsChangedSignal>().RemoveListener(OnPointsChanged);
        Signals.Get<ShakeCoinsSignal>().RemoveListener(ShakeCoins);
    }

    private void Start()
    {
        _floatingTextRt = (RectTransform) _floatingText.transform;
        _floatingText.gameObject.SetActive(false);
        _floatingText2Rt = (RectTransform) _floatingText2.transform;
        _floatingText2.gameObject.SetActive(false);
        _floatingTextOriginalPos = _floatingTextRt.anchoredPosition;

        _lastKnownPoints = GameDataManager.I.Points;
        _text.text = _lastKnownPoints == 0 ? "" :_lastKnownPoints.ToString();
    }

    private void OnPointsChanged()
    {
        _sequence?.Kill();

        var deltaPoints = GameDataManager.I.Points - _lastKnownPoints;
        var deltaKills = GameDataManager.I.Kills - _lastKnownKills;

        var visiblePoints = _lastKnownPoints;
        _lastKnownPoints = GameDataManager.I.Points;
        _lastKnownKills = GameDataManager.I.Kills;

        _floatingText.text = deltaPoints > 0 ?  $"+{deltaPoints}" : "";
        _floatingText.fontSize = 30 + deltaKills * 3;

        _floatingText2.text = GameDataManager.I.LastKillStatus;
        GameDataManager.I.LastKillStatus = "";
        _floatingText2.fontSize = 30 + deltaKills * 3;

        _floatingText.gameObject.SetActive(true);
        _floatingText2.gameObject.SetActive(true);
        _floatingTextRt.localScale = Vector3.zero;
        _floatingText.color = Color.white;
        _floatingText2.color = Color.white;
        _floatingTextRt.anchoredPosition = _floatingTextOriginalPos;

        _sequence = DOTween.Sequence();

        _sequence.AppendCallback(() => { DOVirtual.Float(0, 1, .2f, (percentage) => { _floatingTextRt.localScale = Vector3.one * percentage; }).SetEase(Ease.OutCubic); });
        _sequence.AppendCallback(() => { DOVirtual.Float(0, 1, .2f, (percentage) => { _floatingText2Rt.localScale = Vector3.one * percentage; }).SetEase(Ease.OutCubic); });

        if (deltaPoints > 0)
        {
            _sequence.AppendInterval(1);
        }

        _sequence.AppendCallback(() =>
        {
            if (Player.I.IsDead)
            {
                Hide();
                return;
            }

            DOVirtual.Float(1, 0, 1f, (percentage) => { _floatingTextRt.localScale = Vector3.one * percentage; }).SetEase(Ease.InQuad);

            DOVirtual.Float(0, 1, 1f, (percentage) => { _floatingTextRt.anchoredPosition = _floatingTextOriginalPos + new Vector3(100, 220) * percentage; }).SetEase(Ease.InQuint);

            
                DOVirtual.Float(1, 0, 1, (percentage) =>
                {
                    int p = Mathf.RoundToInt(deltaPoints * percentage);
                    _floatingText.text = deltaPoints > 0 ? $"+{p}": "";
                    _text.text = $"{(visiblePoints + deltaPoints - p)}";
                }).SetEase(Ease.InSine);
            

            _floatingText2.DOColor(new Color(1, 1, 1, 0), 1).SetEase(Ease.InQuad);
        });

        _sequence.AppendInterval(1.1f);
        _sequence.AppendCallback(() =>
        {
            _floatingText.gameObject.SetActive(false);
            _floatingText2.gameObject.SetActive(false);
            _text.text = _lastKnownPoints.ToString();
        });
    }

    private void Hide()
    {
        _sequence?.Kill();
        _floatingText.gameObject.SetActive(false);
        _floatingText2.gameObject.SetActive(false);
        _text.text = GameDataManager.I.Points.ToString();
    }

    private void ShakeCoins()
    {
        ((RectTransform)_text.transform).DOShakeAnchorPos(.4f, new Vector2(10, .5f), 10, 5);
    }

}