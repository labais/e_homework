using System;
using deVoid.Utils;
using DG.Tweening;
using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager I { get; private set; }

    private int _points;

    public int Points
    {
        get => _points;

        private set
        {
            _points = value;
            Signals.Get<PointsChangedSignal>().Dispatch();
        }
    }

    public int LevelNumber { get; set; }

    private int _deadEnemies = 0;
    private int _deadEnemiesBefore = 0;

    // private int _multiplierWindow;
    // private const int _multiplierWindowNominal = 30;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            LevelNumber = 0;
            Signals.Get<EnemyDiedSignal>().AddListener(OnEnemyDied);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        var deltaDeadEnemies = _deadEnemies - _deadEnemiesBefore;
        if (deltaDeadEnemies != 0)
        {
            _deadEnemiesBefore = _deadEnemies;
            int p;
            switch (deltaDeadEnemies)
            {
                case 1:
                    p = 10;
                    break;
                case 2:
                    p = 25;
                    break;
                case 3:
                    p = 45;
                    break;
                case 4:
                    p = 80;
                    break;
                case 5:
                    p = 150;
                    break;
                case 6:
                    p = 250;
                    break;
                default:
                    p = deltaDeadEnemies * 50;
                    break;
            }

            // Debug.Log($"deltaDeadEnemies={deltaDeadEnemies} p={p} frame={Time.frameCount}" );
            Points += p;
        }
    }

    private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        LevelNumber++;
    }

    private void OnEnemyDied()
    {
        _deadEnemies++;
    }

    public void Reset()
    {
        Points = 0;
        LevelNumber = 0;
    }
}