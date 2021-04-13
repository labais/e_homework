using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AgentEffects _agentEffects;
    [SerializeField] private EnemyForwardCollisionChecker _forwardChecker;

    private Mode _mode;
    private float _modeTTL;
    private Vector3 _direction;
    private float _walkTime;
    private Transform _transform;
    private float _speed;
    private float _speedPercentageAtSlowdownStart;
    private const float MaxSpeed = .05f;
    

    private void Start()
    {
        _mode = Mode.Wait;
        _modeTTL = Random.Range(.1f, 1f);
        _transform = transform;
    }

    private void FixedUpdate()
    {
        if ((_modeTTL -= Time.fixedDeltaTime) < 0)
        {
            _mode = (Mode) Random.Range(0, 2+1);
            switch (_mode)
            {
                case Mode.Wait:
                    _modeTTL = Random.Range(.1f, 1f);
                    break;
                case Mode.Wander:
                    _modeTTL = Random.Range(1f, 3f);
                    break;
            }
           
            _direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            _direction.Normalize();
            _forwardChecker.transform.position = _transform.position + _direction * .4f;
            _walkTime = 0;
        }

        if (_mode == Mode.Wander)
        {
            if (_modeTTL > .5f) // @note -- must be inverse of slowdown easing time
            {
                // speedup and move
                _walkTime += Time.fixedDeltaTime;
                _speed = DOVirtual.EasedValue(0, .5f, _walkTime*2, Ease.InSine);
                _transform.position += _direction * (MaxSpeed * _speed);
                _speedPercentageAtSlowdownStart = MaxSpeed / _speed;
            }
            else
            {
                // slowdown
                _speed = DOVirtual.EasedValue(0, _speedPercentageAtSlowdownStart, _modeTTL/2 , Ease.OutSine);
                _transform.position += _direction * (MaxSpeed * _speed);
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _agentEffects.AnimateDeath(AfterDeathAnimation);
        }
    }

    private void AfterDeathAnimation()
    {
        Destroy(this);
    }

    private enum Mode
    {
        Wait,
        Wander,
    }
}