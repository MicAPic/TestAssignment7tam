using System;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] 
    private float damage;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == OwnerClientId) return;
        if (collision.gameObject.TryGetComponent(out HealthController healthController) && 
            healthController.enabled)
        {
            healthController.TakeDamage(damage);
            return;
        }
        DisableServerRpc();
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // allow some time for collision to register server-side to ensure we applied the damage 
        if (collision.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == OwnerClientId) return;
        DisableServerRpc();
    }

    private void OnBecameInvisible()
    {
        DisableServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnableServerRpc(Vector3 direction, float firePower, float damageToDeal)
    {
        EnableClientRpc(direction, firePower, damageToDeal);
    }
    
    [ClientRpc]
    private void EnableClientRpc(Vector3 direction, float firePower, float damageToDeal)
    {
        damage = damageToDeal;
        gameObject.SetActive(true);
        
        _rb.AddForce(direction * firePower, ForceMode2D.Impulse);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableServerRpc()
    {
        DisableClientRpc();
    }
    
    [ClientRpc]
    private void DisableClientRpc()
    {
        gameObject.SetActive(false);
    }
}