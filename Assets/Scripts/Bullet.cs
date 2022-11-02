using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifetime;


    private float _timeLeft;

    public void Init(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public IPooledObject Instantiate(Transform parent)
    {
        return Instantiate(gameObject).GetComponent<Bullet>();
    }

    private void Awake()
    {
        _timeLeft = _lifetime;
    }
    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0)
            Destroy(gameObject);


        float distanceLeft = _speed * Time.deltaTime;

        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.rotation * Vector3.forward;


        while (distanceLeft > 0)
        {
            Ray prediction = new Ray(currentPos, currentDir);
            if (Physics.Raycast(prediction, out RaycastHit hitInfo, distanceLeft))
            {
                //Check for interraction here

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
}
