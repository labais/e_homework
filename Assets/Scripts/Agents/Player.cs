using System;
using deVoid.Utils;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player I { get; private set; }
    public Vector3 InstantMovement{ get; private set; }

    private Transform _transform;
    private Vector3 _lastPos;

    [SerializeField] private AgentEffects _agentEffects;
    
    private void Awake()
    {
        I = this;
        _transform = transform;
    }
    
    private void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
    }

    private void FixedUpdate()
    {
        InstantMovement = _lastPos - _transform.position;
        _lastPos = _transform.position;
    }

    private void OnPlayerDied()
    {
        _agentEffects.AnimateDeath();
    }


}