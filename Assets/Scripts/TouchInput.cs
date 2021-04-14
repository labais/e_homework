using System;
using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour
{
    [SerializeField] private Transform _pointer;


    public Vector3 Target { get; private set; }
    public bool On { get; private set; }
    
    private const int TestGroundLayerMask = 1 << 6;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        var pressPos = Vector2.zero;
        On = false;

        if (Input.touchCount > 0)
        {
            pressPos = Input.touches[0].position;
            On = true;
        }
        else if (Input.GetMouseButton(0))
        {
            pressPos = Input.mousePosition;
            On = true;
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
}