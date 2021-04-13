using System;
using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class EnemyForwardCollisionChecker : MonoBehaviour
{
    public bool IsCool;

    private void Start()
    {
        IsCool = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.LogError($"Enemy {gameObject.name} encountered {other.gameObject}");
        IsCool = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        IsCool = true;
    }

    
}