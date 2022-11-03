using System;
using UnityEngine;


public interface IMove
{
    public event Action<Vector3> OnMove;
}