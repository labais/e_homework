using System;
using deVoid.Utils;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player I { get; private set; }
    public Vector3 InstantMovement { get; private set; }
    public bool IsDead { get; private set; }
    public bool HasMoved { get; private set; }

    private Transform _transform;
    private Vector3 _lastPos;

    [SerializeField] private AgentEffects _agentEffects;

    private void Awake()
    {
        I = this;
        _transform = transform;
        _lastPos = _transform.position;
    }

    private void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
        Signals.Get<PlayerGotHitSignal>().AddListener(OnPlayerGotHit);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
        Signals.Get<PlayerGotHitSignal>().RemoveListener(OnPlayerGotHit);
    }

    private void FixedUpdate()
    {
        InstantMovement = _lastPos - _transform.position;
        _lastPos = _transform.position;
        
        if (!HasMoved && InstantMovement != Vector3.zero)
        {
            HasMoved = true;
            Debug.Log($"lol, moved! {InstantMovement}");
        }
    }

    private void OnPlayerDied()
    {
        _agentEffects.AnimateDeath();
        IsDead = true;
    }

    private void OnPlayerGotHit()
    {
        _agentEffects.PlayHitParticles();
    }
}