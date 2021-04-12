using System;
using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public int LevelNumber { get; set; }
    private int _numTimesLevelLoaded = 0;

    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            LevelNumber = 0;
            Debug.Log($"this one time you know");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
    }

    void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        LevelNumber++;
        _numTimesLevelLoaded++;
        // Debug.Log($"_numTimesLevelLoaded={_numTimesLevelLoaded} arg0={arg0} arg1={arg1}");
        Debug.Log($"OnLevelFinishedLoading LevelNumber={LevelNumber}");
    }

    [Button]
    public void RestartGame()
    {
        LevelNumber = 0;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    [Button]
    public void NexLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}