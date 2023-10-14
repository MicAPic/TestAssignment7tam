using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPool : NetworkBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    private List<Bullet> _pool = new();

    
}
