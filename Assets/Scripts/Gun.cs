using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour, IUsable
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _delay;
    [SerializeField] private GameObject _pooledGameObject;
    private IPooledObject _pooledObject;


    private IObjectPool<IPooledObject> _pool;


    private void Awake()
    {
        if (!_pooledGameObject.TryGetComponent(out _pooledObject))
        {
            Debug.LogError("Gun - IPooledObject Pooled Game Object must have a IPooledObject.");
        }
        _pool = new ObjectPool<IPooledObject>(createFunc: CreatePooledObject);
    }

    public void Use()
    {
        Spawn(_spawnPoint.position, _spawnPoint.rotation);
    }


    private void Spawn(Vector3 position, Quaternion direction)
    {
        var pooledObject = _pool.Get();
        pooledObject.Init(position, direction);
    }

    private IPooledObject CreatePooledObject()
    {
        return _pooledObject.Instantiate(transform);
    }
}