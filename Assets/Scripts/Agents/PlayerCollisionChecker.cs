using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class PlayerCollisionChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Signals.Get<PlayerFinishedSignal>().Dispatch();
        }
        else if (other.CompareTag("Enemy"))
        {
            Signals.Get<PlayerDiedSignal>().Dispatch();
        }
        else
        {
            Signals.Get<PlayerDiedSignal>().Dispatch();
        }
    }
}