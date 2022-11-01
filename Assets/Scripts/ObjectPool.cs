using UnityEngine;
using UnityEngine.Pool;


public class ObjectPool : MonoBehaviour, IPool
{
    [SerializeField] private IPooledObject _objectToPool;


    private IObjectPool<IPooledObject> _pool;


    private void Awake()
    {
        _pool = new ObjectPool<IPooledObject>(createFunc: CreatePooledObject);
    }


    public void Spawn(Vector3 position, Quaternion direction)
    {
        var pooledObject = _pool.Get();
        pooledObject.Init(position, direction);
    }


    private IPooledObject CreatePooledObject()
    {
        return _objectToPool.Instantiate(transform);
    }
}