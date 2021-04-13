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
    private Transform _transform;
    private const float MaxSpeed = .005f;

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
            _mode = (Mode) Random.Range(0, 2 + 1);
            switch (_mode)
            {
                case Mode.Wait:
                    _modeTTL = Random.Range(.1f, 1f);
                    break;
                case Mode.Wander:
                    _modeTTL = Random.Range(1f, 4f);
                    break;
            }

            _direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            _direction.Normalize();
            _forwardChecker.transform.position = _transform.position + _direction * .4f;

            if (!_forwardChecker.IsCool)
            {
                // Debug.Log("PLANNING not cool to walk there");
                _mode = Mode.Wait;
                _modeTTL = .02f;
            }
        }

        if (_mode == Mode.Wander)
        {
            _transform.position += _direction * MaxSpeed;
            if (!_forwardChecker.IsCool)
            {
                // Debug.Log("WALKING not cool to walk there");
                _modeTTL = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _agentEffects.AnimateDeath(AfterDeathAnimation);
        }
        else if (other.CompareTag("Finish"))
        {
        }
        else if (other.CompareTag("Enemy"))
        {
        }
        else if (other.CompareTag("EnemyForwardChecker"))
        {
            // pass
        }
        else
        {
            // doesn't work :\ 
            // fall off from level (not likely, but still) (sides and holes) 
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