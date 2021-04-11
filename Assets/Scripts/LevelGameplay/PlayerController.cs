using System;
using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [FormerlySerializedAs("maxSpeed")] [SerializeField, Range(0f, 100f)]
    float _maxSpeed = 10f;

    [FormerlySerializedAs("maxAcceleration")] [SerializeField, Range(0f, 100f)]
    float _maxAcceleration = 10f;

    [FormerlySerializedAs("maxStickAngle")] [SerializeField, Range(0f, 100f)]
    float _maxStickAngle = 10f;

    [FormerlySerializedAs("stickContainer")] [SerializeField]
    Transform _stickContainer;

    private Vector3 _velocity;
    private Transform _transform;
    private bool _dead;

    private void Start()
    {
        _transform = transform;
    }

    void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }

    void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
    }

    private void FixedUpdate()
    {
        var playerInput = Vector2.zero;
        
        if (!_dead)
        {
            playerInput.x = Input.GetAxisRaw("Horizontal");
            playerInput.y = Input.GetAxisRaw("Vertical");

            #if !UNITY_EDITOR
            playerInput += new Vector2(Random.Range(-1f,1f), 1); // fake input for build
            #endif
            
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        }
        
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;

        float maxSpeedChange = _maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = _velocity * Time.deltaTime;
        Vector3 newPosition = _transform.localPosition + displacement;

        _transform.localPosition = newPosition;

        _stickContainer.eulerAngles = new Vector3(_velocity.z / _maxSpeed * _maxStickAngle, 0, _velocity.x / _maxSpeed * _maxStickAngle);
    }

    private void OnPlayerDied()
    {
        _dead = true;
    }
}