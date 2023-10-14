using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkController : NetworkBehaviour
{
    public static event Action OnPlayerLoaded;
    
    [Header("Shooting")]
    [SerializeField]
    private float fireRate = 1.0f;
    [SerializeField]
    private float firePower = 5.0f;
    [SerializeField]
    private float damageToDeal = 10.0f;

    [SerializeField]
    private Transform shootingPoint;
    
    private float _lastFireTime;
    
    [Space]
    // bullet pooling:
    [SerializeField]
    private GameObject bulletPrefab;
    private List<Bullet> _pool = new();
    //

    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 1.0f;
    [SerializeField]
    private float rotationSpeed = -1.0f;
    private float _movementValue;
    private Vector2 _rotationValue = Vector2.zero;

    [Header("Rotation")]
    [SerializeField]
    private Transform avatar;

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
            _lastFireTime = Time.time;
            GetBulletFromPoolServerRpc();
        }

        // Rotation
        avatar.Rotate(new Vector3(0, 0, _rotationValue.x * rotationSpeed * Time.deltaTime));
    }
    
    void FixedUpdate()
    {
        if (!IsOwner) return;
        
        // Movement
        _rb.velocity = avatar.up * (_movementValue * movementSpeed);
    }

    void OnMove(InputValue value)
    {
        _movementValue = value.Get<float>();
    }

    void OnRotate(InputValue value)
    {
        _rotationValue = value.Get<Vector2>();
    }

    private void Shoot(Bullet bullet)
    {
        bullet.transform.position = shootingPoint.position;
        bullet.EnableServerRpc(avatar.up, firePower, damageToDeal);
    }

    [ServerRpc]
    private void GetBulletFromPoolServerRpc()
    {
        for (var i = 0; i < _pool.Count; i++)
        {
            if (_pool[i].gameObject.activeInHierarchy || _pool[i].OwnerClientId != OwnerClientId) continue;
            
            Shoot(_pool[i]);
            return;
        }

        SpawnBulletServerRpc();
    }

    [ServerRpc]
    private void SpawnBulletServerRpc()
    {
        var result = Instantiate(bulletPrefab, transform).GetComponent<Bullet>();
        result.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        
        _pool.Add(result);
        
        Shoot(result);
    }
}
