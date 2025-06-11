using System.Collections;
using UnityEngine;

public class PullAttack : MonoBehaviour
{
    [Header("Pull Settings")]
    [SerializeField] private float pullRange = 10f;
    [SerializeField] private float pullDuration = 0.2f;
    [SerializeField] private float pullSpeedMultiplier = 1f;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DoPullAttack();
        }
    }

    void DoPullAttack()
    {
        // Find all enemies within range
        Collider[] enemies = Physics.OverlapSphere(transform.position, pullRange, enemyLayer);

        foreach (Collider enemyCollider in enemies)
        {
            Transform enemy = enemyCollider.transform;

            // Apply damage if Enemy script present
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damageAmount);
            }

            // Start coroutine to smoothly pull enemy
            StartCoroutine(PullEnemy(enemy));
        }
    }

    IEnumerator PullEnemy(Transform enemy)
    {
        if (enemy == null) yield break;

        float elapsed = 0f;
        Vector3 startPos = enemy.position;
        Vector3 directionToPlayer = (transform.position - enemy.position).normalized;
        Vector3 targetPos = transform.position - directionToPlayer * 2f;  // 1f = offset distance


        while (elapsed < pullDuration)
        {
            if (enemy == null) yield break;

            enemy.position = Vector3.Lerp(startPos, targetPos, elapsed / pullDuration);
            elapsed += Time.deltaTime * pullSpeedMultiplier;
            yield return null;
        }

        if (enemy != null)
        {
            enemy.position = targetPos;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize pull range in scene view
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRange);
    }
}
