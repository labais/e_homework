using System;
using System.Linq;
using deVoid.Utils;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AgentEffects _agentEffects;
    [SerializeField] private EnemyForwardCollisionChecker _forwardChecker;
    [SerializeField] private LineRenderer _laserLine;

    private Mode _mode;
    private float _modeTTL;
    private Vector3 _direction;
    private Transform _transform;

    private bool _dead;
    // private 

    private const float MaxSpeed = .005f; // @todo -- randomize a little and seldom randomize a lot 
    private const float MinPlayerDistanceToShoot = 10;

    private void Start()
    {
        _mode = Mode.Wait;
        _modeTTL = Random.Range(.1f, 1f);
        _transform = transform;

        _laserLine.gameObject.SetActive(false);
        // var lasetPoints = _laserLine.po
    }

    private void FixedUpdate()
    {
        if (_dead) return;

        if ((_modeTTL -= Time.fixedDeltaTime) < 0)
        {
            _mode = (Mode) Random.Range(0, 2 + 1);

            // @todo -- parametrize this, some guys will be more aggressive
            if (_mode == Mode.Shoot) _mode = (Mode) Random.Range(0, 2 + 1); // randomize again

            // -------------------------------------------------------------------------------------
            // _mode = Mode.Wait; // ------------------------------------------------------------------
            // -------------------------------------------------------------------------------------

            switch (_mode)
            {
                case Mode.Wait:
                    _modeTTL = Random.Range(.01f, .1f);
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
            // laser aim
            _laserLine.gameObject.SetActive(true);
            _laserLine.SetPosition(0, _transform.position + Vector3.up * .7f);
            _laserLine.SetPosition(1, Player.I.transform.position + Vector3.up * .7f);

            if (Vector3.Distance(_transform.position, Player.I.transform.position) > MinPlayerDistanceToShoot)
            {
                _modeTTL = -1; // do something else;
                return;
            }

            if (Player.I.IsDead)
            {
                _modeTTL = -1;
                return;
            }

            // at the end of aiming shoot
            if (_modeTTL <= .1f)
            {
                BulletManager.I.Shoot(_transform.position, Player.I.transform.position, Player.I.InstantMovement);
                _modeTTL = -1;
            }
        }

        if (_mode != Mode.Shoot)
        {
            _laserLine.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _agentEffects.AnimateDeath(AfterDeathAnimation);
            _laserLine.gameObject.SetActive(false);
            Debug.Log("got hit by player", gameObject);
        }
        else if (other.CompareTag("Finish"))
        {
        }
        else if (other.CompareTag("Enemy"))
        {
        }
        else if (other.CompareTag("Bullet"))
        {
        }
        else if (other.CompareTag("Wall"))
        {
            // Very unlikely
            _agentEffects.AnimateDeath(AfterDeathAnimation);
            Debug.Log($"fell off map, lol not jk", gameObject);
        }
        else if (other.CompareTag("EnemyForwardChecker"))
        {
        }
        else
        {
            // Fall in the hole - get vaporized by player 
            _laserLine.gameObject.SetActive(false);
            Debug.Log($"got triggered by {other.name} / {other.tag}  F={Time.frameCount}", gameObject);
            Vaporize();
        }
    }

    private void AfterDeathAnimation()
    {
        Destroy(this);
    }

    public void Vaporize()
    {
        _dead = true;

        Signals.Get<EnemyDiedSignal>().Dispatch();

        AsyncManager.I.Delay(TimeSpan.FromSeconds(HoleAnimator.beamDuration + .01f), () =>
        {
            _agentEffects.AnimateDeath(AfterDeathAnimation);
            _laserLine.gameObject.SetActive(false);
        });

        Debug.Log($"got vaporized F={Time.frameCount}", gameObject);
    }

    private enum Mode
    {
        Wait,
        Wander,
        Shoot,
    }
}