using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class AsyncManager : MonoBehaviour
{
    
    public static AsyncManager I;
    
    private readonly Dictionary<Action, Coroutine> _coroutines = new Dictionary<Action, Coroutine>();
    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();
    
    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    public void Delay(TimeSpan delay, Action action)
    {
        MoveToMainThread(() => { _coroutines[action] = StartCoroutine(DelayCoroutine(delay, action)); });
    }

    public void DelayToNextFrame(Action action)
    {
        MoveToMainThread(() => { _coroutines[action] = StartCoroutine(NextFrameCoroutine(action)); });
    }

    private IEnumerator NextFrameCoroutine(Action action)
    {
        yield return null;
        action();
        RemoveCoroutine(action);
    }

    private IEnumerator DelayCoroutine(TimeSpan delay, Action action)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, (float) delay.TotalSeconds));
        action();
        RemoveCoroutine(action);
    }

    public void CancelCallback(Action action)
    {
        MoveToMainThread(() =>
        {
            if (!_coroutines.ContainsKey(action)) return;

            StopCoroutine(_coroutines[action]);
            RemoveCoroutine(action);
        });
    }

    public void MoveToMainThread(Action action)
    {
        if (action == null) return;
        _mainThreadActions.Enqueue(action);
    }

    private void RemoveCoroutine(Action action)
    {
        if (_coroutines.ContainsKey(action)) _coroutines.Remove(action);
    }

    private void Update()
    {
        if (!_mainThreadActions.IsEmpty)
        {
            while (_mainThreadActions.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }
    }
}