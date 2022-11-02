using UnityEngine;
using UnityEngine.Serialization;

public class Avatar : MonoBehaviour,
    IUsable, IMoving, IRotating, IDamageable
{
    [SerializeField] private GameObject _toolGameObject;
    private IUsable _tool;


    private void Awake()
    {
        if (!_toolGameObject.TryGetComponent(out _tool))
        {
            Debug.LogError("Avatar - IUsable Tool must have a IUsable.");
        }
    }


    public void Move(Vector3 direction)
    {
        //Move
    }
    public void Rotate(Quaternion rotation)
    {
        //Rotate
    }
    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} took {damage} damage");
    }
    public void Use()
    {
        _tool?.Use();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Use();
        }
    }
}
