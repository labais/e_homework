using System;
using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatsPanelBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private float _textSize;
    private Sequence _sequence;

    private void Start()
    {
        _textSize = _text.fontSize;
        ChangeText(false);
    }

    private void OnEnable()
    {
        Signals.Get<UpgradesChangedSignal>().AddListener(OnUpgradesChanged);
    }

    private void OnDisable()
    {
        Signals.Get<UpgradesChangedSignal>().RemoveListener(OnUpgradesChanged);
        _sequence?.Kill();
    }

    private void OnUpgradesChanged()
    {
        ChangeText(true);
    }

    private void ChangeText(bool animate)
    {
        var text = "";
        var n = 0;

        n = GameDataManager.I.GetUpgrade(UpgradeType.ExtraLives);
        if (n > 0)
        {
            text += $"Lives: <color=red><size={_textSize * 1.5f}>";
            for (var i = 0; i < n; i++) text += "•";
            text += $"</size></color>   \n";
        }

        n = GameDataManager.I.GetUpgrade(UpgradeType.Speed);
        if (n > 0)
        {
            text += $"Speed: lvl {n + 1}\n";
        }

        n = GameDataManager.I.GetUpgrade(UpgradeType.TrailLength);
        if (n > 0)
        {
            text += $"Trail: lvl {n + 1}\n";
        }

        n = GameDataManager.I.GetUpgrade(UpgradeType.ImmunityBalls);
        if (n > 0)
        {
            text += $"Balls can't hurt you\n";
        }

        n = GameDataManager.I.GetUpgrade(UpgradeType.ImmunityTrailCut);
        if (n > 0)
        {
            text += $"Trail cannot be cut\n";
        }

        n = GameDataManager.I.GetUpgrade(UpgradeType.ImmunityEnemies);
        if (n > 0)
        {
            text += $"You can touch enemies\n";
        }

        if (animate)
        {
            _text.text = "";

            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(.5f);

            var skipAnimate = 0;
            var foundIt = false;
            foreach (var letter in text)
            {
                if (!foundIt && letter == '<')
                {
                    foundIt = true;
                    skipAnimate = 45; // hack: cheaply skip rich text stuff
                }

                if (skipAnimate > 0)
                {
                    skipAnimate--;
                }
                else
                {
                    _sequence.AppendInterval(.01f);
                }

                _sequence.AppendCallback(() =>
                {
                    _text.text += letter;
                });
            }
        }
        else
        {
            _text.text = text;
        }
    }
}