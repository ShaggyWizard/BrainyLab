using UnityEngine;


public interface IPool
{
    public void Spawn(Vector3 position, Quaternion direction);
}