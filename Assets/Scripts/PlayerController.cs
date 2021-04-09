using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;
    [SerializeField, Range(0f, 100f)] float maxStickAngle = 10f;
    [SerializeField] Transform stickContainer;
   
    
    Vector3 velocity;

    void Start()
    {
    }

    void FixedUpdate()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // Debug.Log($"{playerInput}");

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;
        

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

        transform.localPosition = newPosition;
        
        // Debug.Log($"x% = {velocity.x/maxSpeed} velocity.x={velocity.x} maxSpeed={maxSpeed}");

        stickContainer.eulerAngles = new Vector3(velocity.z/maxSpeed *maxStickAngle, 0,  velocity.x/maxSpeed *maxStickAngle );
    }
    
    
    
}