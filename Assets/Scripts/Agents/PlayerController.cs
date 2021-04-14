using deVoid.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _maxAcceleration = 10f;
    [SerializeField] float _maxStickAngle = 10f;
    [SerializeField] Transform _stickContainer;
    [SerializeField] TouchInput _touchInput;

    private Vector3 _velocity;
    private Transform _transform;
    private bool _dead;

    private bool _finished;
    private int _finishedAutoMoveTTL;
    private Vector3 _lastInput;
    private const int FinishedAutoMoveTTLNominal = 60;

    private bool _firstInputRecieved;
    private InputMethod _selectedInputMethod = InputMethod.None;
    

    private void Start()
    {
        _transform = transform;
    }

    void OnEnable()
    {
        Signals.Get<PlayerDiedSignal>().AddListener(OnPlayerDied);
        Signals.Get<PlayerFinishedSignal>().AddListener(OnPlayerFinished);
    }

    void OnDisable()
    {
        Signals.Get<PlayerDiedSignal>().RemoveListener(OnPlayerDied);
        Signals.Get<PlayerFinishedSignal>().RemoveListener(OnPlayerFinished);
    }

    private void FixedUpdate()
    {
        var playerInput = Vector2.zero;

        if (!_dead && !_finished)
        {
            var gamepad = Gamepad.current;
            if (gamepad != null && _selectedInputMethod != InputMethod.Touch)
            {
                // Read onscreen gamepad
                playerInput = gamepad.leftStick.ReadValue();

                if (_selectedInputMethod != InputMethod.Gamepad && playerInput.magnitude  > 0.05f)
                {
                    _touchInput.Disable();
                    _selectedInputMethod = InputMethod.Gamepad;
                }

            }

            // Read keyboard
            playerInput += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Read touch input
            if (_touchInput.Started && _selectedInputMethod != InputMethod.Gamepad)
            {
                var distanceToTouchTarget = _touchInput.Target - _transform.position;
                if (distanceToTouchTarget.magnitude > .5f)
                {
                    playerInput += new Vector2(distanceToTouchTarget.normalized.x, distanceToTouchTarget.normalized.z);
                }
                
                if (_selectedInputMethod != InputMethod.Touch)
                {
                    Signals.Get<DisableOnScreenGamepad>().Dispatch();
                    _selectedInputMethod = InputMethod.Touch;
                }
            }

            if (!_firstInputRecieved && playerInput.magnitude > .3f)
            {
                _firstInputRecieved = true;
                Signals.Get<PlayerStartedControllingSignal>().Dispatch();
            }

            playerInput = Vector2.ClampMagnitude(playerInput, 1f);
            _lastInput = playerInput;
        }

        if (_finished && _finishedAutoMoveTTL-- > 0)
        {
            // continue last input before finishing, but slow it down
            playerInput = _lastInput * Mathf.Lerp(.1f, 0, 1 - ((float) _finishedAutoMoveTTL / FinishedAutoMoveTTLNominal));
        }

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;

        float maxSpeedChange = _maxAcceleration * Time.fixedDeltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = _velocity * Time.fixedDeltaTime;
        Vector3 newPosition = _transform.localPosition + displacement;

        _transform.localPosition = newPosition;

        _stickContainer.eulerAngles = new Vector3(_velocity.z / _maxSpeed * _maxStickAngle, 0, _velocity.x / _maxSpeed * _maxStickAngle);
    }

    private void OnPlayerDied()
    {
        _dead = true;
    }

    private void OnPlayerFinished()
    {
        _finished = true;
        _finishedAutoMoveTTL = FinishedAutoMoveTTLNominal;
    }

    private enum InputMethod
    {
        None,
        Touch,
        Gamepad,
    }
}