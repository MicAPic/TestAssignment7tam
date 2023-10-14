using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] 
    private float damage;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == OwnerClientId) return;
        if (col.gameObject.TryGetComponent(out HealthController healthController) && 
            healthController.enabled)
        {
            _spriteRenderer.enabled = false;
            healthController.TakeDamage(damage);
            return;
        }
        DisableServerRpc();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // allow some time for collision to register server-side to ensure we applied the damage 
        if (other.gameObject.GetComponent<NetworkBehaviour>().OwnerClientId == OwnerClientId) return;
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
        _spriteRenderer.enabled = true;
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