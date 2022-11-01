using UnityEngine;

public class Avatar : MonoBehaviour,
    IUsable, IMoving, IRotating, IDamageable
{
    [SerializeField] private IUsable _tool;

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
}
