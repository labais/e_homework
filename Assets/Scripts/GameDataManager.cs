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

    public int Kills { get; private set; }
    public string LastKillStatus { get; private set; }
    
    public int LevelNumber { get; private set; }
    public int NumberOfHoles { get; private set; }

    public float TotalTrailLength { get; set; }

    private int _deadEnemies = 0;
    private int _deadEnemiesBefore = 0;
    private bool _goldenKIll;


    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            LevelNumber = 0;
            Signals.Get<EnemyDiedSignal>().AddListener(OnEnemyDied);
            Signals.Get<HoleGeneratedSignal>().AddListener(OnHoleGenerated);
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
            Kills += deltaDeadEnemies;
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


            LastKillStatus = "";
            if (deltaDeadEnemies > 1) LastKillStatus = "Multikill!";
            if (deltaDeadEnemies > 3) LastKillStatus = "Megakill!";
            if (deltaDeadEnemies > 4) LastKillStatus = "Hyperkill!!!";
            //@todo golden     

            if (_goldenKIll)
            {
                p *= 2;
                LastKillStatus = $"Golden {LastKillStatus}";
            }
    
            

            Points += p;
        }
    }

    private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        LevelNumber++;
    }

    private void OnEnemyDied(bool golden)
    {
        _deadEnemies++;
        _goldenKIll = golden;
    }

    private void OnHoleGenerated(Transform _, Transform __)
    {
        NumberOfHoles++;
    }

    public void Reset()
    {
        Points = 0;
        LevelNumber = 0;
        NumberOfHoles = 0;
        _deadEnemies = 0;
        _deadEnemiesBefore = 0;
        TotalTrailLength = 0;
    }
}