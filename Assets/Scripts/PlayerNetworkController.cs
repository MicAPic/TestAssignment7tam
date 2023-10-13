using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkController : NetworkBehaviour
{
    public static event Action OnPlayerLoaded;
    
    [Header("Shooting")]
    [SerializeField]
    public float fireRate = 1.0f;
    private float _lastFireTime;
    
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 1.0f;
    [SerializeField]
    private float rotationSpeed = -1.0f;
    private float _movementValue;
    private Vector2 _rotationValue = Vector2.zero;

    private Rigidbody2D _rb;
    private PlayerInput _playerInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        OnPlayerLoaded?.Invoke();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        
        // Shooting
        if (_playerInput.actions["Fire"].IsPressed() && Time.time - _lastFireTime >= fireRate)
        {
            Shoot();
        }

        // Rotation
        transform.Rotate(new Vector3(0, 0, _rotationValue.x * rotationSpeed));
    }
    
    void FixedUpdate()
    {
        // Movement
        _rb.velocity = transform.up * (_movementValue * movementSpeed);
    }

    void OnMove(InputValue value)
    {
        _movementValue = value.Get<float>();
    }

    void OnRotate(InputValue value)
    {
        _rotationValue = value.Get<Vector2>();
    }

    private void Shoot()
    {
        Debug.Log("Shot fired!");
        _lastFireTime = Time.time;
    }
}
