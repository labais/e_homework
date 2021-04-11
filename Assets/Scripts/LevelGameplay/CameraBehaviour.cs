using deVoid.Utils;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private bool _dead;
    private float _xPos;
    private float _yPos;

    void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
    }

    void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
    }

    private void Start()
    {
        _xPos = _player.position.x;
        _yPos = _player.position.y;
        transform.position = _player.position;
    }

    void FixedUpdate()
    {
        if (!_dead)
        {
            // slowly creep towards player but never move back
            var move = Mathf.Lerp(transform.position.z, _player.position.z, .005f);
            if (move > transform.position.z)
            {
                transform.position = new Vector3(_xPos, _yPos, move);
            }
        }
        else
        {
            // Move to death location
            var move = Vector3.Lerp(transform.position, _player.position, .005f);
            transform.position = move;
        }
    }

    private void OnPlayerDied()
    {
        _dead = true;
    }
}