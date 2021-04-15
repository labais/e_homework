using deVoid.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _stickContainer;
    [SerializeField] private TouchInput _touchInput;

    private const int FinishedAutoMoveTTLNominal = 60;
    private const float MaxAcceleration = 64;
    private const float MaxStickAngle = 10f;

    private Vector3 _velocity;
    private Transform _transform;
    private bool _dead;
    private bool _finished;
    private int _finishedAutoMoveTTL;
    private Vector3 _lastInput;
    private float _maxSpeedBase = 7;
    private float _maxSpeed;


    private bool _firstInputReceived;
    private InputMethod _selectedInputMethod = InputMethod.None;

    private void Start()
    {
        _transform = transform;
        _maxSpeed = _maxSpeedBase + (GameDataManager.I.GetUpgrade(UpgradeType.Speed) * .6f);

        Debug.Log($"Maxspeed={_maxSpeed} sec lvl={GameDataManager.I.GetUpgrade(UpgradeType.Speed)}");
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

                if (_selectedInputMethod != InputMethod.Gamepad && playerInput.magnitude > 0.05f)
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

            if (!_firstInputReceived && playerInput.magnitude > .1f)
            {
                _firstInputReceived = true;
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

        float maxSpeedChange = MaxAcceleration * Time.fixedDeltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = _velocity * Time.fixedDeltaTime;
        Vector3 newPosition = _transform.localPosition + displacement;

        _transform.localPosition = newPosition;

        _stickContainer.eulerAngles = new Vector3(_velocity.z / _maxSpeed * MaxStickAngle, 0, _velocity.x / _maxSpeed * MaxStickAngle);
    }

    private void OnPlayerDied()
    {
        SoundManager.I.Play("death", .3f, true);
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