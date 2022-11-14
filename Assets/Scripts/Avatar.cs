using System;
using UnityEditor;
using UnityEngine;



public class Avatar : MonoBehaviour, IDamageable, ITrajectory, IScore, ISender, IColor
{
    [Header("Settings")]
    [SerializeField, Min(0f)] private float _speed;

    [Header("Dependencies")]
    [SerializeField] private Renderer _colorRenderer;
    [SerializeField] private GameObject _toolGameObject;

    [Header("Inputs")]
    [SerializeField] private GameObject _userGameObject;
    [SerializeField] private GameObject _moveGameObject;
    [SerializeField] private GameObject _rotateGameObject;


    public event Action<int> OnScoreChange;

    public Vector3 Position => transform.position;
    public Vector3 Velocity => _rigidbody.velocity;
    public Color Color => _colorRenderer == null ? Color.white : _colorRenderer.material.color;
    public ISender Sender => this;
    public int Score
    {
        get { return _score; }
        set 
        {
            _score = value;
            OnScoreChange?.Invoke(value);
        }
    }


    private IUsable _tool;
    private IUser _user;
    private IMove _move;
    private ILook _rotate;

    private Rigidbody _rigidbody;

    private int _score;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        bool fail = false;
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

        if (_toolGameObject.TryGetComponent(out _tool))
        {
            ISender sender = _tool as ISender;
            if (sender != null)
            {
                sender.SetSender(Sender);
            }
        }

        _user.OnUse += Use;
        _move.OnMove += Move;
        _rotate.OnLook += LookAt;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_rigidbody == null) { return; }

        Handles.color = Color.red; 
        Handles.DrawLine(transform.position, transform.position + _rigidbody.velocity.normalized);
        Handles.color = Color.green;
        Handles.DrawLine(transform.position, transform.position + (transform.rotation * Vector3.forward));
    }
#endif


    public void TakeDamage(float damage, ISender sender = null)
    {
        Debug.Log($"{name} took {damage} damage");

        if (sender == null) return;


        IScore senderScore = sender as IScore;
        if (sender != null)
        {
            senderScore.AddScore( sender == Sender ? -1 : 1);
        }
    }
    public void SetSender(ISender sender) { }


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
    public void AddScore(int amount)
    {
        Score += amount;
    }
}
