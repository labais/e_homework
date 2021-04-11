using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class FinishBehaviour : MonoBehaviour
{
    void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(OnPlayerFinished);
    }

    void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(OnPlayerFinished);
    }
    
    private void OnPlayerFinished()
    {
        
    }
    
}
