using deVoid.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ControlScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private Image[] _imagesToFade;

    private void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(Stop);
        Signals.Get<PlayerDiedSignal>().AddListener(Stop);
        Signals.Get<PlayerStartedControllingSignal>().AddListener(OnPlayerStartedControlling);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(Stop);
        Signals.Get<PlayerDiedSignal>().RemoveListener(Stop);
        Signals.Get<PlayerStartedControllingSignal>().RemoveListener(OnPlayerStartedControlling);
    }

    private void Awake()
    {
        _content.SetActive(true);
    }

    private void Stop()
    {
        _content.SetActive(false);
    }

    private void OnPlayerStartedControlling()
    {
        foreach (var image in _imagesToFade)
        {
            var color = image.color;
            color.a = .05f;
            image.DOColor(color, 1).SetEase(Ease.InCirc).SetDelay(1);
        }
    }
}