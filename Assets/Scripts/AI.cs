using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AI : MonoBehaviour, IMove, ILook, IUser
{
    [SerializeField] private NavMeshAgent _agent;


    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnLook;
    public event Action OnUse;


    private Vector3 _direction;


    private void Update()
    {

    }
    private void FixedUpdate()
    {
        OnMove?.Invoke(_direction * Time.fixedDeltaTime);
    }
}
