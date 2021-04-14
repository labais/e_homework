using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool Free { get; private set; } // Free for grabs in bullet pool

    private float _ttl;
    private Vector3 _direction;
    private Transform _transform;
    private float _speed;

    private const float NormalSpeedA = .18f;
    private const float NormalSpeedB = .23f;
    private const float TTLNominal = 5;

    private void Awake()
    {
        _transform = transform;
    }

    public void Shoot(Vector3 start, Vector3 target, Vector3 instantMovement)
    {
        const int hitInFrames = 30;
        Free = false;
        transform.position = start;
        gameObject.SetActive(true);
        _ttl = TTLNominal;
        var believedLocationInFuture = target - instantMovement * hitInFrames; // where would player be in X frames
        var distance = (believedLocationInFuture - start).magnitude;
        _speed = distance / hitInFrames; // it would be better to calculate correct angle and not change the speed, but this will suffice
        _speed = Mathf.Clamp(_speed, NormalSpeedA, NormalSpeedB); // don't let the speed get ridiculous

        _direction = (believedLocationInFuture - start).normalized;
    }

    private void FixedUpdate()
    {
        if (Free)
        {
            gameObject.SetActive(false);
            return;
        }

        if ((_ttl -= Time.fixedDeltaTime) > 0)
        {
            _transform.position += _direction * _speed;
        }
        else
        {
            Free = true;
        }
    }

    public void Hit()
    {
        // move away from camera, let trail disappear, then disable, not the best solution, but will do
        transform.position = Vector3.back * 1000;
        AsyncManager.I.Delay(TimeSpan.FromSeconds(2), ()=>{
            gameObject.SetActive(false);
            Free = true;
        });
        
    }
}