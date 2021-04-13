using deVoid.Utils;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AgentEffects _agentEffects;
    
    
    private void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
    }


    
    private void OnPlayerDied()
    {
        _agentEffects.AnimateDeath();
    }


}