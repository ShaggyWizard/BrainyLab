using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour, IUsable, ITrajectory, ISender
{
    [Header("Settings")]
    [SerializeField, Min(0f)] private float _useCooldown;

    [Header("Objects")]
    [SerializeField] private GameObject _pooledGameObject;
    [SerializeField] private Transform _spawnPoint;


    private IPooledObject _pooledObject;
    private IObjectPool<IPooledObject> _pool;
    private float _canUseTime;


    public Vector3 Position => _spawnPoint.position;
    public Vector3 Velocity => (_pooledObject is ITrajectory) ? _spawnPoint.rotation * ((ITrajectory)_pooledObject).Velocity : Vector3.zero;
    public ISender Sender { get; private set; }


    private void Awake()
    {
        if (!_pooledGameObject.TryGetComponent(out _pooledObject))
        {
            Debug.LogError("Gun - IPooledObject Pooled Game Object must have a IPooledObject.");
        }
        _pool = new ObjectPool<IPooledObject>(CreatePooledObject, OnGetPooledObject, OnReleasePooledObject, OnDestroyPooledObject);
    }


    public void Use()
    {
        if (Time.time < _canUseTime) { return; }

        _canUseTime = Time.time + _useCooldown;
        _pool.Get();
    }


    private IPooledObject CreatePooledObject()
    {
        var obj = _pooledObject.Instantiate(transform);
        obj.OnRelease += (s) => _pool.Release(s);

        return obj;
    }
    private void OnGetPooledObject(IPooledObject obj)
    {
        ISender sender = obj as ISender;
        if (sender != null)
        {
            sender.SetSender(Sender);
        }
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

    public void SetSender(ISender sender)
    {
        Sender = sender;
    }
}