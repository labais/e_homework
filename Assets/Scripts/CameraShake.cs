using deVoid.Utils;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeMagnitude = 0.1f;
    [SerializeField] private float _dampingSpeed = 1.0f;

    private Transform _transform;
    private float _shakeDuration;
    Vector3 _initialPosition;

    void OnEnable()
    {
        Signals.Get<ShakeCameraSignal>().AddListener(OnShakeCamera);
    }

    void OnDisable()
    {
        Signals.Get<ShakeCameraSignal>().RemoveListener(OnShakeCamera);
    }

    private void Start()
    {
        _transform = transform;
        _initialPosition = _transform.localPosition;
    }

    void FixedUpdate()
    {
        if (_shakeDuration > 0)
        {
            _transform.localPosition = _initialPosition + Random.insideUnitSphere * _shakeMagnitude;
            _shakeDuration -= Time.deltaTime * _dampingSpeed;
        }
    }

    private void OnShakeCamera(float duration, float magnitude)
    {
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
    }
}