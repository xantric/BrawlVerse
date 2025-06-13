using UnityEngine;

public class Headbutt : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode attackKey = KeyCode.T;
    public float attackRange = 1.5f;
    public int damage = 10;
    public float cooldown = 1.5f;
    
    [Header("References")]
    public Transform attackOrigin;
    public LayerMask enemyLayer;

    private float nextAttackTime;
    private Animator animator;
    private bool isHeadbutting = false;

    void Start()
    {
        // Fix: Get Animator from parent instead of self
        animator = GetComponentInParent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found in parent! Headbutt animation won't play.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey) && Time.time >= nextAttackTime && !isHeadbutting)
        {
            StartHeadbutt();
        }
    }

    void StartHeadbutt()
    {
        isHeadbutting = true;
        animator.SetBool("isHeadbutting", true); // Make sure casing matches Animator exactly
        nextAttackTime = Time.time + cooldown;
    }

    // Called via Animation Event at the impact frame
    public void ApplyHeadbuttDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackOrigin.position, attackRange, enemyLayer);
        
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    // Called via Animation Event at the end of the animation
    public void EndHeadbutt()
    {
        isHeadbutting = false;
        animator.SetBool("isHeadbutting", false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
        }
    }
}
