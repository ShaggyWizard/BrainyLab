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
        _pool = new ObjectPool<IPooledObject>(CreatePooledObject, OnGetPooledObject, OnReleasePooledObject, OnDestroyPooledObject, false, 10, 1000);
    }


    public void Use()
    {
        _pool.Get();
    }


    private IPooledObject CreatePooledObject()
    {
        var obj = _pooledObject.Instantiate(transform);
        obj.OnRelease += (s) => _pool.Release(s); ;
        return obj;
    }
    private void OnGetPooledObject(IPooledObject obj)
    {
        obj.OnPoolGet(_spawnPoint.position, _spawnPoint.rotation);
    }
    private void OnReleasePooledObject(IPooledObject obj)
    {
        obj.OnPoolRelease();
    }
    private void OnDestroyPooledObject(IPooledObject obj)
    {
        obj.OnPoolDestroy();
    }
}