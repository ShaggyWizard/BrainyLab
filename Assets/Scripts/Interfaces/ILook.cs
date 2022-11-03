using System;
using UnityEngine;


public interface ILook
{
    public event Action<Vector3> OnLook;
}