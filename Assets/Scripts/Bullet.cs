using System;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject, ITrajectory, ISender
{
    [Header("Debug")]
    [SerializeField, Min(0)] private float _gizmoLineLength;

    [Header("Settings")]
    [SerializeField, Min(0)] private float _damage;
    [SerializeField, Min(0)] private float _speed;


    public event Action<IPooledObject> OnRelease;
    public Vector3 Velocity => transform.rotation * Vector3.forward * _speed;
    public Vector3 Position => transform.position;

    public ISender Sender { get; private set; }

    public IPooledObject Instantiate(Transform parent)
    {
        return Instantiate(gameObject).GetComponent<Bullet>();
    }


    private void Update()
    {
        float distanceLeft = _speed * Time.deltaTime;

        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.rotation * Vector3.forward;


        while (distanceLeft > 0)
        {
            Ray prediction = new Ray(currentPos, currentDir);
            if (Physics.Raycast(prediction, out RaycastHit hitInfo, distanceLeft) && !hitInfo.transform.TryGetComponent(out IPassThrough pass))
            {
                if (CheckCollision(hitInfo)) { return; }

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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.white;

        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.rotation * Vector3.forward;
        float distanceLeft = _gizmoLineLength;

        while (distanceLeft > 0)
        {
            Ray prediction = new Ray(currentPos, currentDir);
            if (Physics.Raycast(prediction, out RaycastHit hitInfo, distanceLeft) && !hitInfo.transform.TryGetComponent(out IPassThrough pass))
            {
                Handles.DrawLine(currentPos, hitInfo.point);

                if (CheckCollision(hitInfo, true)) { return; }
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
#endif


    public void OnPoolGet(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
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
            if (onlyCheck) { return true; }

            damageable.TakeDamage(_damage, Sender);
            OnRelease.Invoke(this);

            return true;
        }

        return false;
    }

    public void SetSender(ISender sender)
    {
        Sender = sender;
    }
}
