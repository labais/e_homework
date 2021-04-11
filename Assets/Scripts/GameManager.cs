using System;
using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    private int _numTimesLevelLoaded = 0;
    

    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void OnDestroy()
    {
        Debug.Log("lol, destroyed!");
    }

    void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        _numTimesLevelLoaded++;
        Debug.Log($"_numTimesLevelLoaded={_numTimesLevelLoaded} arg0={arg0} arg1={arg1}");
    }
    
    [Button]
    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    
}