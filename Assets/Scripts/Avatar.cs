using UnityEditor;
using UnityEngine;

public class Avatar : MonoBehaviour, IDamageable, ITrajectory
{
    [Header("Settings")]
    [SerializeField, Min(0f)] private float _speed;

    [Header("Dependencies")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _toolGameObject;

    [Header("Inputs")]
    [SerializeField] private GameObject _userGameObject;
    [SerializeField] private GameObject _moveGameObject;
    [SerializeField] private GameObject _rotateGameObject;


    private IUsable _tool;
    private IUser _user;
    private IMove _move;
    private ILook _rotate;


    public Vector3 Position => transform.position;
    public Vector3 Velocity => _rigidbody.velocity;


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

    private void OnDrawGizmos()
    {
        Handles.color = Color.red; 
        Handles.DrawLine(transform.position, transform.position + _rigidbody.velocity.normalized);
        Handles.color = Color.green;
        Handles.DrawLine(transform.position, transform.position + (transform.rotation * Vector3.forward));
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} took {damage} damage");
    }


    private void Use()
    {
        _tool?.Use();
    }
    private void Move(Vector3 moveDelta)
    {
        _rigidbody.velocity = moveDelta * _speed;
    }
    private void LookAt(Vector3 point)
    {
        transform.LookAt(point);
    }
}
