using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class PlayerCollisionChecker : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log($"PlayerCollisionChecker::FINISHED!");
            Signals.Get<PlayerFinishedSignal>().Dispatch();
        }
        else
        {
            Debug.Log($"PlayerCollisionChecker::player's dead, prolly");
            Signals.Get<PlayerDiedSignal>().Dispatch();
        }

        
    }
}