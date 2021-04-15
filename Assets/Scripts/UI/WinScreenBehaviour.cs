using System.Runtime.CompilerServices;
using deVoid.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private TextMeshProUGUI _text1;
    [SerializeField] private Button _buttonNextLevel;
    [SerializeField] private Animator _screenAnimator;
    [SerializeField] private OfferButtonBehaviour _offerButton;

    private static readonly int Open = Animator.StringToHash("Open");

    private void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(OnPlayerFinished);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(OnPlayerFinished);
    }

    private void Awake()
    {
        _content.SetActive(false);
        _buttonNextLevel.onClick.AddListener(OnNextLevelClick);
    }

    private void OnNextLevelClick()
    {
        GameManager.I.NexLevel();
    }

    private void OnPlayerFinished()
    {
        SoundManager.I.Play("win", 3f, true);
        
        _content.SetActive(true);
        _text1.text = $"Level {GameDataManager.I.LevelNumber.ToString("D2")} finished!";
        _screenAnimator.SetTrigger(Open);

        var type = GetRandomUpgradeToBuyThatIsNotAlreadyUpgradedToMaxLevel();
        var price = GameDataManager.I.GetUpgradePriceForNextLevelOfType(type);
        var actualPrice = price;
        var r = Random.Range(0, 1f);
        if (r < .04f)
        {
            actualPrice = price / 5;
        }
        else if (r < .1f)
        {
            actualPrice = price / 2;
        }

        _offerButton.Redraw(new OfferButtonData()
        {
            UpgradeType = type,
            Price = price,
            ActualPrice = actualPrice,
            CanBuy = actualPrice <= GameDataManager.I.Points,
        }, Buy);
    }

    private void Buy(UpgradeType boughtType, int price)
    {
        GameDataManager.I.IncrementUpgrade(boughtType);
        GameDataManager.I.LastKillStatus = Random.Range(0, 10 + 1) == 0 ? "Go nuts!" : "Have fun!";
        GameDataManager.I.Points -= price;
    }

    private UpgradeType GetRandomUpgradeToBuyThatIsNotAlreadyUpgradedToMaxLevel()
    {
        for (var i = 0; i < 10; i++)
        {
            var t = GetRandomUpgradeToBuy();
            if (!GameDataManager.I.IsUpgradeAtMaxLevel(t))
            {
                return t;
            }
        }

        return UpgradeType.None;
    }

    private UpgradeType GetRandomUpgradeToBuy()
    {
        if (Random.Range(0, 10 + 1) == 0)
        {
            Debug.Log($"tough titties, you get no offer!");
            return UpgradeType.None;
        }

        if (Random.Range(0, 10 + 1) < GameDataManager.I.LevelNumber)
        {
            Debug.Log($"simple offer");
            return Random.Range(0, 1 + 1) == 0 ? UpgradeType.Speed : UpgradeType.TrailLength;
        }
        else
        {
            Debug.Log($"great offer");
            return (UpgradeType) Random.Range((int) UpgradeType.Speed, GameDataManager.I.NumUpgrades);
        }
    }
}