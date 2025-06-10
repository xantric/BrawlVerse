using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;           // Assign in inspector
    public int kickDamage = 20;         
    public float kickRange = 0.1f;      
    public LayerMask enemyLayer;        
    public Transform kickPoint;
    public GameObject kickHitbox;

    public void EnableKickHitbox()
    {
        kickHitbox.SetActive(true);
    }

    public void DisableKickHitbox()
    {
        kickHitbox.SetActive(false);
    } // ✅ This was missing!

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))  // Change this to your kick input
        {   
            Kick();
        }
    }

    void Kick()
    {
        animator.SetTrigger("Kick");
        // Damage is applied later in animation event PlayerKickFunction()
    }

    // Called by animation event
    public void PlayerKickFunction()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(kickPoint.position, kickRange, enemyLayer);

        foreach (Collider enemyCollider in hitEnemies)
        {
            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(kickDamage);
                Debug.Log($"Damaged {enemyCollider.name} for {kickDamage} points");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (kickPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(kickPoint.position, kickRange);
    }
}
