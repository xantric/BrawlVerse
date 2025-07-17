using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class EnemyAI : MonoBehaviourPun
{
    public Transform player;
    public float pushForce = 10f;
    public float knockbackTime = 0.5f;
    public float fallThreshold = -10f;
    public Vector2 respawnAreaX = new Vector2(-5f, 5f);
    public Vector2 respawnAreaZ = new Vector2(-5f, 5f);
    public int scoreReward = 10;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isKnockedBack = false;

    private GameObject lastHitByPlayer; // Track who hit the enemy last

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            GiveScoreToLastPlayer();
            Respawn();
            return;
        }

        if (!isKnockedBack && player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = (collision.transform.position - transform.position).normalized;
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
                playerRb.AddForce(pushDir * pushForce, ForceMode.Impulse);

            lastHitByPlayer = collision.gameObject; // Track who hit us last
        }
    }

    public void Knockback(Vector3 force, GameObject sourcePlayer)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;
        agent.enabled = false;
        rb.AddForce(force, ForceMode.Impulse);
        lastHitByPlayer = sourcePlayer;
        Invoke(nameof(RecoverFromKnockback), knockbackTime);
    }

    void RecoverFromKnockback()
    {
        isKnockedBack = false;
        agent.enabled = true;
    }

    void Respawn()
    {
        float x = Random.Range(respawnAreaX.x, respawnAreaX.y);
        float z = Random.Range(respawnAreaZ.x, respawnAreaZ.y);
        transform.position = new Vector3(x, 3f, z);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isKnockedBack = false;
        agent.enabled = true;
    }

    void GiveScoreToLastPlayer()
    {
        if (lastHitByPlayer == null) return;

        if (lastHitByPlayer.TryGetComponent(out ScoreManager scoreManager))
        {
            scoreManager.AddScore(scoreReward);
        }

        lastHitByPlayer = null; // Reset
    }

    // Call this from your player's attack code, e.g., push attack or punch
    public void OnHitByPlayer(Vector3 direction, float force, GameObject sourcePlayer)
    {
        Knockback(direction.normalized * force, sourcePlayer);
    }
}
