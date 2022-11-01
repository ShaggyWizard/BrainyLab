using UnityEngine;

public class Gun : MonoBehaviour, IUsable
{
    [SerializeField] private Transform _spawnPoint;


    [SerializeField] private IPool _pool;


    public void Use()
    {
        _pool.Spawn(_spawnPoint.position, _spawnPoint.rotation);
    }
}