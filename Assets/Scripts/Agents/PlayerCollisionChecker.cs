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
            if (GameDataManager.I.GetUpgrade(UpgradeType.ImmunityEnemies) > 0)
            {
                return;
            }

            Signals.Get<PlayerDiedSignal>().Dispatch();
        }
        else if (other.CompareTag("EnemyForwardChecker")) // It would be much better to use separate physics layers, but this will do for now 
        {
            // pass
        }
        else if (other.CompareTag("Bullet"))
        {
            SoundManager.I.Play("hit", .2f, true);
            Signals.Get<PlayerGotHitSignal>().Dispatch();
            other.GetComponent<Bullet>()?.Hit();
        }
        else
        {
            Signals.Get<PlayerDiedSignal>().Dispatch();
        }
    }
}