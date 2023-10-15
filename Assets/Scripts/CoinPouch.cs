using Unity.Netcode;

public class CoinPouch : NetworkBehaviour
{
    public NetworkVariable<int> coins = new(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );

    public void AddToPouch(int amountToAdd)
    {
        if (!IsOwner) return;
        coins.Value += amountToAdd;
    }
}
