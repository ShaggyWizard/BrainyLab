using UnityEngine;


public interface IPooledObject
{
    public IPooledObject Instantiate(Transform parent);
    public void Init(Vector3 position, Quaternion rotation);
}