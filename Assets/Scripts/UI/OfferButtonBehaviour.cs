using System;
using deVoid.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferButtonBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _price1;
    [SerializeField] private TextMeshProUGUI _price2;
    [SerializeField] private GameObject _slashMark;

    private bool _done;
    private Action<UpgradeType, int> _afterBuyCallback;
    private OfferButtonData _data;
    private Vector3 _originalPosition;

    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (!_data.CanBuy)
        {
            Signals.Get<ShakeCoinsSignal>().Dispatch();
            return;
        }

        if (_done) return;

        _done = true;
        _afterBuyCallback(_data.UpgradeType, _data.ActualPrice);
        gameObject.SetActive(false);
    }

    public void Redraw(OfferButtonData data, Action<UpgradeType, int> afterBuyCallback)
    {
        if (data.UpgradeType == UpgradeType.None)
        {
            gameObject.SetActive(false);
            return;
        }

        _afterBuyCallback = afterBuyCallback;
        _data = data;
        switch (data.UpgradeType)
        {
            case UpgradeType.Speed:
                _text.text = $"Upgrade speed to level {(GameDataManager.I.Upgrades[(int) UpgradeType.Speed] + 2).ToString()}";
                break;
            case UpgradeType.TrailLength:
                _text.text = $"Upgrade trail length to level {(GameDataManager.I.Upgrades[(int) UpgradeType.Speed] + 2).ToString()}";
                break;
            case UpgradeType.ExtraLives:
                _text.text = $"Buy a bonus life";
                break;
            case UpgradeType.ImmunityBalls:
                _text.text = $"Buy immunity from balls";
                break;
            case UpgradeType.ImmunityTrailCut:
                _text.text = $"Buy immunity from trail cuts";
                break;
            case UpgradeType.ImmunityEnemies:
                _text.text = $"Buy immunity from enemies";
                break;
        }

        _price1.text = $"${data.Price}";
        _price2.text = $"${data.ActualPrice}";
        _price2.gameObject.SetActive(data.ActualPrice != data.Price);
        _slashMark.gameObject.SetActive(data.ActualPrice != data.Price);
    }
}

public struct OfferButtonData
{
    public int Price;
    public int ActualPrice;
    public bool CanBuy;
    public UpgradeType UpgradeType;
}