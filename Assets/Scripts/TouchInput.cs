using System;
using UnityEngine;
using System.Collections;

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

    void FixedUpdate()
    {
        if (_disabled) return;
        
        
        RaycastHit hit;

        var pressPos = Vector2.zero;
        Started = false;

        if (Input.touchCount > 0)
        {
            pressPos = Input.touches[0].position;
            Started = true;
        }
        else if (Input.GetMouseButton(0))
        {
            pressPos = Input.mousePosition;
            Started = true;
        }
        else
        {
            return;
        }

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