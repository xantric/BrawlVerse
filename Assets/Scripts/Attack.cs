using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    [SerializeField] public PlayerMovement playerMovement;

    public float damagePerSecond = 20f;
    public float damageRadius = 1.5f; // Radius of the roll hit
    public LayerMask enemyLayer; // Only affect enemies

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    public void GiveDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        if (playerMovement != null && playerMovement.isRolling)
        {
            DetectAndDamageEnemies();
        }
    }

    void DetectAndDamageEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, damageRadius, enemyLayer);

        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("enemy"))
            {
                Attack enemyAttack = enemy.GetComponent<Attack>();
                if (enemyAttack != null)
                {
                    int damage = Mathf.RoundToInt(damagePerSecond * Time.deltaTime);
                    enemyAttack.GiveDamage(damage);
                    Debug.Log($"Rolling hit: {enemy.name} took {damage} damage.");
                }
            }
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add player death logic here
    }

    // Optional: visualize the sphere in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
