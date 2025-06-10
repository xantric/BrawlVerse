using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickHitbox : MonoBehaviour
{
    [Tooltip("Amount of damage to deal on kick hit.")]
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log($"Hit enemy {other.name} with kick, dealing {damage} damage.");
                health.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning($"Enemy {other.name} has no Health component.");
            }
        }
    }
}
