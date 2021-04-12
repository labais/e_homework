using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private TextMeshProUGUI _text1;
    [SerializeField] private Button _buttonNextLevel;

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
        _content.SetActive(true);
        _text1.text = $"Level {GameManager.I.LevelNumber.ToString("D2")} finished!";
    }
}