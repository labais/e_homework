using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsDisplayBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private int _lastKnownPoints = 0;
    
    private void OnEnable()
    {
        Signals.Get<PointsChangedSignal>().AddListener(OnPointsChanged);
    }

    private void OnDisable()
    {
        Signals.Get<PointsChangedSignal>().RemoveListener(OnPointsChanged);
    }

    private void Start()
    {
        _lastKnownPoints = GameDataManager.I.Points;
        _text.text = _lastKnownPoints.ToString();
    }

    private void OnPointsChanged()
    {
        var delta = GameDataManager.I.Points - _lastKnownPoints;
        _text.text = GameDataManager.I.Points.ToString();
    }

}