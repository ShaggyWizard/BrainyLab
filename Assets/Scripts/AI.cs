using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public class AI : MonoBehaviour, IMove, ILook, IUser
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GameObject _targetGameObject;
    [SerializeField] private GameObject _toolGameObject;
    [SerializeField] private float _maxDistance;


    private ITrajectory _target;
    private ITrajectory _tool;
    private NavMeshPath _pathToMove;
    private NavMeshPath _pathToAim;


    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnLook;
    public event Action OnUse;


    private Vector3 _direction;
    private Vector3 _aimTarget;


    private void Awake()
    {
        _targetGameObject.TryGetComponent(out _target);
        _toolGameObject.TryGetComponent(out _tool);

        _pathToMove = new NavMeshPath();
        _pathToAim = new NavMeshPath();
    }
    private void Update()
    {
        _agent.CalculatePath(_target.Position, _pathToMove);

        Vector3 pointPos = _pathToMove.corners[1];
        if (_pathToMove.corners.Length == 2)
            pointPos += (transform.position - _target.Position).normalized * _maxDistance;
        _direction = (pointPos - transform.position).normalized;
    }
    private void LateUpdate()
    {
        Aim();
        TryShoot();
    }
    private void FixedUpdate()
    {
        OnMove?.Invoke(_direction * Time.fixedDeltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_tool == null || _target == null) { return; }

        Vector3 target = AcquireTargetLock(_target);
        Handles.color = Color.red;

        Handles.DrawWireDisc(target, Vector3.up, 1);

        Handles.DrawLine(transform.position, target);
    }
#endif


    private void Aim()
    {
        if (_tool == null || _target == null) { return; }

        _aimTarget = AcquireTargetLock(_target);

        OnLook?.Invoke(_aimTarget);
    }
    private void TryShoot()
    {
        _agent.CalculatePath(_aimTarget, _pathToAim);
        if (_pathToAim.corners.Length <= 2)
        {
            OnUse?.Invoke();
        }
    }
    private Vector3 AcquireTargetLock(ITrajectory target)
    {
        float toolOffset = Vector3.Distance(transform.position, new Vector3(_tool.Position.x, transform.position.y, _tool.Position.z));

        var projectileSpeed = _tool.Velocity.magnitude;
        float a = (target.Velocity.x * target.Velocity.x) + (target.Velocity.z * target.Velocity.z) - (projectileSpeed * projectileSpeed);

        float b = 2 * (target.Velocity.x * (target.Position.x - transform.position.x) + target.Velocity.z * (target.Position.z - transform.position.z));

        float c = ((target.Position.x - transform.position.x) * (target.Position.x - transform.position.x)) +
            ((target.Position.z - transform.position.z) * (target.Position.z - transform.position.z)) - toolOffset;

        float disc = b * b - (4 * a * c);
        if (disc < 0)
        {
            Debug.LogError("No possible hit!");
            return _target.Position;
        }
        else
        {
            float t1 = (-1 * b + Mathf.Sqrt(disc)) / (2 * a);
            float t2 = (-1 * b - Mathf.Sqrt(disc)) / (2 * a);
            float t = Mathf.Max(t1, t2);// let us take the larger time value 
            float aimX = (target.Velocity.x * t) + target.Position.x;
            float aimZ = target.Position.z + (target.Velocity.z * t);
            return new Vector3(aimX, 0, aimZ);//now position the turret
        }
    }
}
