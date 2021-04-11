using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    

    private void Start()
    {
        _transform = transform;
    }

    private void FixedUpdate()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // Debug.Log($"{playerInput}");

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;

        float maxSpeedChange = _maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = _velocity * Time.deltaTime;
        Vector3 newPosition = _transform.localPosition + displacement;

        // if (newPosition.x < allowedArea.xMin) {
        //     newPosition.x = allowedArea.xMin;
        //     velocity.x = -velocity.x * bounciness;
        // }
        // else if (newPosition.x > allowedArea.xMax) {
        //     newPosition.x = allowedArea.xMax;
        //     velocity.x = -velocity.x * bounciness;
        // }
        // if (newPosition.z < allowedArea.yMin) {
        //     newPosition.z = allowedArea.yMin;
        //     velocity.z = -velocity.z * bounciness;
        // }
        // else if (newPosition.z > allowedArea.yMax) {
        //     newPosition.z = allowedArea.yMax;
        //     velocity.z = -velocity.z * bounciness;
        // }


        _transform.localPosition = newPosition;
      
        _stickContainer.eulerAngles = new Vector3(_velocity.z / _maxSpeed * _maxStickAngle, 0, _velocity.x / _maxSpeed * _maxStickAngle);
    }
}