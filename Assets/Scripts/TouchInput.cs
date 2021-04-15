using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using deVoid.Utils;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
    [SerializeField] private Transform _pointer;

    public Vector3 Target { get; private set; }
    public bool Started { get; private set; }

    private const int TestGroundLayerMask = 1 << 6;
    private Camera _camera;
    private bool _disabled;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        Signals.Get<PlayerFinishedSignal>().AddListener(Disable);
        Signals.Get<PlayerDiedSignal>().AddListener(Disable);
    }

    private void OnDisable()
    {
        Signals.Get<PlayerFinishedSignal>().RemoveListener(Disable);
        Signals.Get<PlayerDiedSignal>().RemoveListener(Disable);
    }

    void FixedUpdate()
    {
        if (_disabled) return;

        RaycastHit hit;

        var pressPos = Vector2.zero;
        Started = false;

        if (Input.touchCount > 0)
        {
            pressPos = Input.touches[0].position;
        }
        else if (Input.GetMouseButton(0))
        {
            pressPos = Input.mousePosition;
        }
        else
        {
            return;
        }

        // Dirty haxxx:
        // Ignore input in bottom of the screen - give chance to onscreen gamepad
        if (!Player.I.HasMoved && pressPos.y < Screen.height / 3f)
        {
            return;
        }

        Started = true;

        var ray = _camera.ScreenPointToRay(pressPos);
        if (Physics.Raycast(ray, out hit, float.MaxValue, TestGroundLayerMask))
        {
            _pointer.transform.position = hit.point;
            Target = hit.point;
        }
    }

    public void Disable()
    {
        _disabled = true;
        _pointer.gameObject.SetActive(false);
    }
}