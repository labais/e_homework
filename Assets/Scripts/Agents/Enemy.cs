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
    private bool _dead;

    private const float MaxSpeed = .005f; // @todo -- randomize a little and seldom randomize a lot 
    private const float MinPlayerDistanceToShoot = 10;

    private void Start()
    {
        _mode = Mode.Wait;
        _modeTTL = Random.Range(.1f, 1f);
        _transform = transform;
    }

    private void FixedUpdate()
    {
        if (_dead) return;

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
                case Mode.Shoot:
                    _modeTTL = 1;
                    break;
            }

            _direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            _direction.Normalize();
            _forwardChecker.transform.position = _transform.position + _direction * .4f;

            if (!_forwardChecker.IsCool)
            {
                _mode = Mode.Wait;
                _modeTTL = .02f;
            }
        }

        if (_mode == Mode.Wander)
        {
            _transform.position += _direction * MaxSpeed;
            if (!_forwardChecker.IsCool)
            {
                _modeTTL = 0;
            }
        }
        else if (_mode == Mode.Shoot)
        {
            if (Vector3.Distance(transform.position, Player.I.transform.position) > MinPlayerDistanceToShoot)
            {
                _modeTTL = -1; // do something else;
                return;
            }

            // at the end of aiming shoot
            if (_modeTTL <= .1f)
            {
                BulletManager.I.Shoot(transform.position, Player.I.transform.position, Player.I.InstantMovement);
                _modeTTL = -1;
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

    public void Vaporize()
    {
        _dead = true;
        Debug.LogError("@odo -- add points for killing enemy!");

        AsyncManager.I.Delay(TimeSpan.FromSeconds(HoleAnimator.beamDuration + .01f), () => { _agentEffects.AnimateDeath(AfterDeathAnimation); });
    }

    private enum Mode
    {
        Wait,
        Wander,
        Shoot,
    }
}