using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private NetworkVariable<float> healthPoints = new(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField]
    private float hitInvincibilityTime = 0.15f;
    private bool _canTakeDamage = true;

    private float _maxHealth;

    // [Header("Visuals")]
    // [SerializeField]
    // private GameObject healthBar;
    // [SerializeField]
    // private Image healthBarFill;

    void Awake()
    {
        _maxHealth = healthPoints.Value;
    }

    public void TakeDamage(float damagePoints)
    {
        if (!_canTakeDamage) return;
        
        healthPoints.Value -= damagePoints;
        if (healthPoints.Value <= 0)
        {
            Die();
        }
        StartCoroutine(ToggleInvincibility());

        // if (!healthBar.activeInHierarchy)
        // {
        //     // like in VS: health bar isn't active until we take damage
        //     healthBar.SetActive(true);
        // }
        //
        // healthBarFill.fillAmount = healthPoints / _maxHealth;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    
    private IEnumerator ToggleInvincibility()
    {
        _canTakeDamage = false;
        yield return new WaitForSeconds(hitInvincibilityTime);
        _canTakeDamage = true;
    }
}
