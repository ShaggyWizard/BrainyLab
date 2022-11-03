using UnityEngine;
using UnityEngine.Serialization;

public class Avatar : MonoBehaviour,
    IUsable, IDamageable
{
    [Header("Settings")]
    [SerializeField, Min(0f)] private float _speed;

    [Header("Dependencies")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private MonoBehaviour _toolGameObject;

    [Header("Inputs")]
    [SerializeField] private MonoBehaviour _moveGameObject;
    [SerializeField] private MonoBehaviour _rotateGameObject;


    private IUsable _tool;
    private IMove _move;
    private ILook _rotate;


    private void Awake()
    {
        bool fail = false;
        if (!_rigidbody)
        {
            fail = true;
            Debug.LogError("Avatar - Must have Rigidbody.");
        }
        if (!_moveGameObject.TryGetComponent(out _move))
        {
            fail = true;
            Debug.LogError("Avatar - Move GameObject must have a IMove.");
        }
        if (!_rotateGameObject.TryGetComponent(out _rotate))
        {
            fail = true;
            Debug.LogError("Avatar - Rotate GameObject must have a IRotate.");
        }

        if (fail)
        {
            Destroy(this);
            return;
        }

        _move.OnMove += Move;
        _rotate.OnLook += LookAt;
    }


    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} took {damage} damage");
    }
    public void Use()
    {
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
