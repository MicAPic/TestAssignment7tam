using Unity.Netcode;
using UnityEngine;

public class Coin : Interactable
{
    [SerializeField]
    private int value = 1;

    private bool _isInteractable = true;
    
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (_isInteractable && col.gameObject.TryGetComponent(out CoinPouch pouch))
        {
            _isInteractable = false;
            pouch.AddToPouch(value);
            DisableServerRpc();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // sometimes the collision doesn't register properly, so we do this just to make sure
        OnTriggerEnter2D(other);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}