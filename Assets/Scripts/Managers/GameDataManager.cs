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

        set
        {
            _points = value;
            Signals.Get<PointsChangedSignal>().Dispatch();
        }
    }

    public int Kills { get; private set; }
    public string LastKillStatus { get; set; }
    public int LevelNumber { get; private set; }
    public int NumberOfHoles { get; private set; }
    public float TotalTrailLength { get; set; }
    
    private int _deadEnemies = 0;
    private int _deadEnemiesBefore = 0;
    private bool _goldenKIll;

    private int[] _upgrades;
    private readonly int[] _maxLevelForUpgrades = GenerateMaxUpgradeLevelData();
    public readonly int NumUpgrades = Enum.GetNames(typeof(UpgradeType)).Length;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
           
            Signals.Get<EnemyDiedSignal>().AddListener(OnEnemyDied);
            Signals.Get<HoleGeneratedSignal>().AddListener(OnHoleGenerated);
            Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);

            ResetData();
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

    private void OnPlayerDied()
    {
        if (!Player.I.HasMoved)
        {
            Signals.Get<MustShowRestartSignal>().Dispatch(true);
        }
        else if (GetUpgrade(UpgradeType.ExtraLives) > 0)
        {
            _upgrades[(int) UpgradeType.ExtraLives]--;
            Signals.Get<MustShowRestartSignal>().Dispatch(false);
        }
        else
        {
            Signals.Get<MustShowGameOverSignal>().Dispatch();
        }
    }

    private void OnHoleGenerated(Transform _, Transform __)
    {
        NumberOfHoles++;
    }

    public void ResetData()
    {
        Points = 0;
        LevelNumber = 0;
        NumberOfHoles = 0;
        _deadEnemies = 0;
        _deadEnemiesBefore = 0;
        TotalTrailLength = 0;
        LevelNumber = 0;
        _upgrades = new int[NumUpgrades];
    }

    public void IncrementUpgrade(UpgradeType type)
    {
        _upgrades[(int) type]++;
        Signals.Get<UpgradesChangedSignal>().Dispatch();
    }

    public int GetUpgrade(UpgradeType type)
    {
        return _upgrades[(int) type];
    }

    public bool IsUpgradeAtMaxLevel(UpgradeType type)
    {
        return _upgrades[(int) type] >= _maxLevelForUpgrades[(int) type];
    }

    public int GetUpgradePriceForNextLevelOfType(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed:
                return (_upgrades[(int) UpgradeType.Speed] + 1) * 20;
            case UpgradeType.TrailLength:
                return (_upgrades[(int) UpgradeType.TrailLength] + 1) * 20;
            case UpgradeType.ExtraLives:
                return 100 + (_upgrades[(int) UpgradeType.ExtraLives] + 1) * 200;
            case UpgradeType.ImmunityBalls:
                return 500;
            case UpgradeType.ImmunityTrailCut:
                return 500;
            case UpgradeType.ImmunityEnemies:
                return 2000;
        }

        return 0;
    }

    private static int[] GenerateMaxUpgradeLevelData()
    {
        return new[]
        {
            0,
            19,
            19,
            2,
            1,
            1,
            1,
        };
    }
    
    [Button]
    public void GiveMoney()
    {
        Points += 1000;
    }
}

public enum UpgradeType
{
    None,
    Speed,
    TrailLength,
    ExtraLives,
    ImmunityBalls,
    ImmunityTrailCut,
    ImmunityEnemies,
}