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
        else if (other.CompareTag("EnemyForwardChecker")) // It would be much better to use separate physics layers, but this will do for now 
        {
            // pass
        }
        else if (other.CompareTag("Bullet"))
        {
            Debug.Log("can't hurt me nuttin");
            Signals.Get<PlayerGotHitSignal>().Dispatch();
        }
        else
        {
            Signals.Get<PlayerDiedSignal>().Dispatch();
        }
    }
}