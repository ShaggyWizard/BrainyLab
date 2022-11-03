using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IMove, ILook
{
    [SerializeField] private float _mouseLookPlaneHeight;


    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnLook;


    private Vector3 _direction;
    private Plane _plane;

    private void Awake()
    {
        _plane = new Plane(Vector3.up, _mouseLookPlaneHeight);
    }

    private void Update()
    {
        _direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_plane.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);

            if (point == Vector3.zero) { return; }

            OnLook?.Invoke(point);
        }
    }
    private void FixedUpdate()
    {
        OnMove?.Invoke(_direction * Time.fixedDeltaTime);
    }
}
