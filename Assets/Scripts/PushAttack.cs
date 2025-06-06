using UnityEngine;

public class PushAttack : MonoBehaviour
{
    public float pushRadius = 2f;         // Radius in which enemies are pushed
    public float pushForce = 500f;        // How strong the push is
    public LayerMask enemyLayer;          // Define which objects are "enemies"
    public Transform pushOrigin;          // Point from where push is applied (like player's position)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DoPushAttack();
        }
    }

    void DoPushAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(pushOrigin.position, pushRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = (enemy.transform.position - pushOrigin.position).normalized;
                rb.AddForce(pushDirection * pushForce);
            }
        }

        Debug.Log("Push attack triggered!");
    }

    // Optional: visualize push radius in Scene view
    void OnDrawGizmosSelected()
    {
        if (pushOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pushOrigin.position, pushRadius);
    }
}
