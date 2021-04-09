using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private void Start()
    {
        transform.position = Vector3.forward * _player.position.z;
    }

    void FixedUpdate()
    {
        // slowly creep towards player but never move back
        var move = Mathf.Lerp(transform.position.z, _player.position.z, .01f);
        if (move > transform.position.z)
        {
            transform.position = Vector3.forward * move;
        }
    }
}