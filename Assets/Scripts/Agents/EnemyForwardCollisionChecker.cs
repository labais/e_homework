using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class EnemyForwardCollisionChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError($"Enemy {gameObject.name} encountered {other.gameObject}");
    }
}