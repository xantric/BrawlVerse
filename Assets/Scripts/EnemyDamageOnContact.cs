using UnityEngine;

public class EnemyDamageOnContact : MonoBehaviour
{
    public float damagePerSecond = 10f;    // Damage applied per second when near player
    public float damageInterval = 1f;      // How often damage applies (in seconds)

    private float damageTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                // Get player's health script
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerSecond);
                }

                damageTimer = 0f;  // reset timer
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer = 0f; // reset timer when player leaves
        }
    }
}
