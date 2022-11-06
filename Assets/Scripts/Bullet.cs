using System;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject, ITrajectory
{
    [Header("Debug")]
    [SerializeField] private float _gizmoLineLength;

    [Header("Settings")]
    [SerializeField, Min(0)] private float _damage;
    [SerializeField, Min(0)] private float _speed;
    [SerializeField, Min(0)] private float _lifetime;
    [SerializeField, Min(0)] private int _maxBounces;


    public event Action<IPooledObject> OnRelease;
    public Vector3 Velocity => transform.rotation * Vector3.forward * _speed;
    public Vector3 Position => transform.position;


    private float _timeLeft;
    private int _bounceCount;


    public IPooledObject Instantiate(Transform parent)
    {
        return Instantiate(gameObject).GetComponent<Bullet>();
    }


    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0)
        {
            OnRelease.Invoke(this);
            return;
        }


        float distanceLeft = _speed * Time.deltaTime;

        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.rotation * Vector3.forward;


        while (distanceLeft > 0)
        {
            Ray prediction = new Ray(currentPos, currentDir);
            if (Physics.Raycast(prediction, out RaycastHit hitInfo, distanceLeft))
            {
                if (CheckCollision(hitInfo)) { return; }

                _bounceCount--;
                if (_bounceCount < 0)
                {
                    OnRelease.Invoke(this);
                    return;
                }

                distanceLeft -= (hitInfo.point - currentPos).magnitude;
                currentPos = hitInfo.point;
                currentDir = Vector3.Reflect(currentDir, hitInfo.normal);
            }
            else
            {
                currentPos += currentDir * distanceLeft;
                break;
            }
        }

        transform.position = currentPos;
        transform.rotation = Quaternion.LookRotation(currentDir);
    }
    private void OnDrawGizmos()
    {
        Handles.color = Color.white;

        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.rotation * Vector3.forward;
        float distanceLeft = _speed * _timeLeft;

        int predictionBounceCount = _bounceCount;

        while (distanceLeft > 0)
        {
            Ray prediction = new Ray(currentPos, currentDir);
            if (Physics.Raycast(prediction, out RaycastHit hitInfo, distanceLeft))
            {
                Handles.DrawLine(currentPos, hitInfo.point);

                if (CheckCollision(hitInfo, true)) { return; }

                predictionBounceCount--;
                if (predictionBounceCount < 0)
                {
                    return;
                }

                distanceLeft -= (hitInfo.point - currentPos).magnitude;
                currentPos = hitInfo.point;
                currentDir = Vector3.Reflect(currentDir, hitInfo.normal);
            }
            else
            {
                Handles.DrawLine(currentPos, currentPos + currentDir * distanceLeft);
                break;
            }
        }
    }


    public void OnPoolGet(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        _timeLeft = _lifetime;
        _bounceCount = _maxBounces;
        gameObject.SetActive(true);
    }
    public void OnPoolRelease()
    {
        gameObject.SetActive(false);
    }
    public void OnPoolDestroy()
    {
        Destroy(gameObject);
    }


    private bool CheckCollision(RaycastHit hitInfo, bool onlyCheck = false)
    {
        if (hitInfo.transform.TryGetComponent(out IDamageable damageable))
        {
            if (!onlyCheck)
            {
                damageable.TakeDamage(_damage);
                OnRelease.Invoke(this);
            }
            return true;
        }

        return false;
    }
}
