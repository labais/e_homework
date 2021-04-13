using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AgentEffects _agentEffects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _agentEffects.AnimateDeath(AfterDeathAnimation);
        }
    }

    private void AfterDeathAnimation()
    {
        Destroy(this);
    }
}