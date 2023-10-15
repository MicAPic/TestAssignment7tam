using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : NetworkBehaviour
{
    protected abstract void OnTriggerEnter2D(Collider2D col);
}