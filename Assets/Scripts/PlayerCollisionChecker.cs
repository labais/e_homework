using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionChecker : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.LogError($"PlayerCollisionChecker::FINISHED!");
        }
        else
        {
            Debug.LogError($"PlayerCollisionChecker::player's dead, prolly");
        }

        
    }
}