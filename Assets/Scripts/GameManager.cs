using deVoid.Utils;
using DG.Tweening;
using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    
    private int _numTimesLevelLoaded = 0;
    

    private void Awake()
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

    private void OnLevelFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        _numTimesLevelLoaded++;
        // Debug.Log($"_numTimesLevelLoaded={_numTimesLevelLoaded} arg0={arg0} arg1={arg1}");
        // Debug.Log($"OnLevelFinishedLoading LevelNumber={LevelNumber}");
    }

    [Button]
    public void RestartGame()
    {
        DOTween.KillAll();
        GameDataManager.I.Reset();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    [Button]
    public void NexLevel()
    {
        DOTween.KillAll();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    
    
}