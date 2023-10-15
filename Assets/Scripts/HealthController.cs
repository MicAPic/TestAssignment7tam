using UI;
using Unity.Netcode;
using UnityEngine;

public class HealthController : NetworkBehaviour
{
    [Header("Parameters")]
    public NetworkVariable<float> healthPoints = new(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> maxHealth;
    
    [Header("Visuals")]
    [SerializeField]
    private Transform damageParticles;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = damageParticles.GetComponent<ParticleSystem>();
    }

    public override void OnNetworkSpawn()
    {
        maxHealth = new NetworkVariable<float>(
            healthPoints.Value, 
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
            );
    }

    public void TakeDamage(float damagePoints, Vector3 damagePoint)
    {
        damageParticles.position = damagePoint;
        _particleSystem.Play();
        
        if (!IsOwner) return;
        
        healthPoints.Value -= damagePoints;
        
        if (healthPoints.Value <= 0)
        {
            // OnPlayerDeath?.Invoke(OwnerClientId);
            DieServerRpc();
        }
    }

    [ServerRpc]
    private void DieServerRpc()
    {
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        transform.parent.gameObject.SetActive(false);
        UIManager.Instance.RemovePlayerInfoClientRpc(OwnerClientId);
    }
}
