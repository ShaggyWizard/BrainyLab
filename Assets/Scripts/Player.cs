using System;
using UnityEngine;


public class Player : MonoBehaviour, IMove, ILook, IUser
{
    [SerializeField] private float _mouseLookPlaneHeight;


    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnLook;
    public event Action OnUse;


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

        if (Input.GetMouseButton(0))
        {
            OnUse?.Invoke();
        }
    }
    private void FixedUpdate()
    {
        OnMove?.Invoke(_direction * Time.fixedDeltaTime);
    }
}
