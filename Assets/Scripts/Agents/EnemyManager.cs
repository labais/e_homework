using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using deVoid.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    public static EnemyManager I { get; private set; }
    public List<Enemy> Enemies;

    private const float MinDistanceFromNextEnemy = .9f;

    private static int _unqID = 1;

    private void OnEnable()
    {
        Signals.Get<LevelGeneratedSignal>().AddListener(OnLevelGenerated);
    }

    private void OnDisable()
    {
        Signals.Get<LevelGeneratedSignal>().RemoveListener(OnLevelGenerated);
    }

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        _prefab.SetActive(false);
    }

    private void OnLevelGenerated(float levelSizeX, float levelSizeZ)
    {
        // @todo -- parametrise depending on GameManager.I.LevelNumber 
        var chunks =Random.Range(2, 10);
        var maxNumPerChunk =Random.Range(1, 8);

        Enemies = new List<Enemy>();

        // Debug.Log($"EnemyManager::chunks={chunks}");
        // Debug.Log($"EnemyManager::maxNumPerChunk={maxNumPerChunk}");

        for (var i = 0; i < chunks; i++)
        {
            var chunkPosition = Vector3.zero;
            var foundEmptySpot = false;
            for (var r = 0; r < 15; r++)
            {
                //retries to randomly find empty spot
                chunkPosition = GetRandomPointOnLevel(levelSizeX, levelSizeZ);
                var distance = GetDistanceToClosestEnemy(chunkPosition);
                if (distance > 4)
                {
                    foundEmptySpot = true;
                    break;
                }
            }

            if (!foundEmptySpot)
            {
                // Debug.Log("no free space to spawn enemy chunk!");
                break;
            }

            var numPerChunk = (Random.Range(1, maxNumPerChunk) + Random.Range(1, maxNumPerChunk)) / 2;

            for (var j = 0; j < numPerChunk; j++)
            {
                var enemyPosition = chunkPosition;

                for (var r = 0; r < 5; r++)
                {
                    foundEmptySpot = false;
                    enemyPosition = chunkPosition + new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f));
                    var distance = GetDistanceToClosestEnemy(chunkPosition);
                    if (distance > MinDistanceFromNextEnemy)
                    {
                        foundEmptySpot = true;
                        break;
                    }
                }

                if (!foundEmptySpot)
                {
                    // Debug.Log($"EnemyManager::chunk[{i}] enemy[{j}] chunk pos={chunkPosition} No free space to put enemy");
                    continue;
                }

                // Debug.Log($"EnemyManager::chunk[{i}] enemy[{j}] chunk pos={chunkPosition} enemy pos={enemyPosition}");
                var enemy = Instantiate(_prefab, enemyPosition, Quaternion.identity, transform);
                enemy.name = $"Enemy {_unqID++}";
                Enemies.Add(enemy.GetComponent<Enemy>());
                enemy.SetActive(true);
            }
        }
    }

    Vector3 GetRandomPointOnLevel(float levelSizeX, float levelSizeZ)
    {
        var numPerChunk = Random.Range(1, 3);
        var posX = Random.Range(2, levelSizeX - 2);
        var posZ = Random.Range(6, levelSizeZ - 5);
        return new Vector3(posX, 0, posZ);
    }

    private float GetDistanceToClosestEnemy(Vector3 pos)
    {
        var closestDist = float.MaxValue;
        float d;

        foreach (var enemy in Enemies)
        {
            if (enemy == null) continue;

            d = Vector3.Distance(pos, enemy.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                // Debug.DrawLine(pos + Vector3.up, enemy.transform.position, Color.magenta, 999);
            }
        }

        // also check player
        d = Vector3.Distance(pos, Player.I.transform.position);
        if (d < closestDist)
        {
            closestDist = d;
            // Debug.DrawLine(pos + Vector3.up, Player.I.transform.position, Color.magenta, 999);
        }

        return closestDist;
    }
}