using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public ShieldController shieldController;  // Reference to your existing shield script

    void Start()
    {
        if (shieldController == null)
        {
            shieldController = GetComponent<ShieldController>(); // Try auto-assign if on same object
        }
    }

    public void TakeDamage(float damageAmount)
    {
        // If shield active, ignore damage
        if (shieldController != null && shieldController.IsShieldActive)
        {
            Debug.Log("Shield is active! No damage taken.");
            return;
        }

        health -= damageAmount;
        Debug.Log("Player health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Your death logic here
    }
}
