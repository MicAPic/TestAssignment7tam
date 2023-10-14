using Unity.Netcode;
using UnityEngine;

public class HealthController : NetworkBehaviour
{
    [Header("Parameters")]
    public NetworkVariable<float> healthPoints = new(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> maxHealth;

    public override void OnNetworkSpawn()
    {
        maxHealth = new NetworkVariable<float>(
            healthPoints.Value, 
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
            );
    }

    public void TakeDamage(float damagePoints)
    {
        if (!IsOwner) return;
        
        healthPoints.Value -= damagePoints;
        
        if (healthPoints.Value <= 0)
        {
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
    }
}
