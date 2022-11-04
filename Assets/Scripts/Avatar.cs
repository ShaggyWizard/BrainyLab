using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Avatar : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField, Min(0f)] private float _speed;
    [SerializeField, Min(0f)] private float _useCooldown;

    [Header("Dependencies")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private MonoBehaviour _toolGameObject;

    [Header("Inputs")]
    [SerializeField] private MonoBehaviour _userGameObject;
    [SerializeField] private MonoBehaviour _moveGameObject;
    [SerializeField] private MonoBehaviour _rotateGameObject;


    private IUsable _tool;
    private IUser _user;
    private IMove _move;
    private ILook _rotate;

    private float _canUseTime;


    private void Awake()
    {
        bool fail = false;
        if (!_rigidbody)
        {
            fail = true;
            Debug.LogError("Avatar - Must have Rigidbody.");
        }
        if (!_userGameObject || !_userGameObject.TryGetComponent(out _user))
        {
            fail = true;
            Debug.LogError("Avatar - User GameObject must have a IUser.");
        }
        if (!_moveGameObject || !_moveGameObject.TryGetComponent(out _move))
        {
            fail = true;
            Debug.LogError("Avatar - Move GameObject must have a IMove.");
        }
        if (!_rotateGameObject || !_rotateGameObject.TryGetComponent(out _rotate))
        {
            fail = true;
            Debug.LogError("Avatar - Rotate GameObject must have a IRotate.");
        }

        if (fail)
        {
            Destroy(this);
            return;
        }

        _toolGameObject.TryGetComponent(out _tool);

        _user.OnUse += Use;
        _move.OnMove += Move;
        _rotate.OnLook += LookAt;
    }


    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} took {damage} damage");
    }


    private void Use()
    {
        if (Time.time < _canUseTime) { return; }

        _canUseTime = Time.time + _useCooldown;
        _tool?.Use();
    }
    private void Move(Vector3 moveDelta)
    {
        _rigidbody.velocity = moveDelta * _speed;
    }
    private void LookAt(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction = new Vector3(direction.x, 0, direction.z);
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
