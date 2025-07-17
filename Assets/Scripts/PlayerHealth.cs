using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
{
    public float health = 100f;
    public float maxHealth = 100f;
    public Slider healthBar;

    public PlayerStateMachine _playerStateMachine;  
    public bool isLocalPlayer;

    void Start()
    {
        if (_playerStateMachine == null)
            _playerStateMachine = GetComponent<PlayerStateMachine>();

        if (isLocalPlayer && healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }
    }

    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        if (_playerStateMachine != null && _playerStateMachine.isShieldActive)
        {
            Debug.Log("Shield is active! No damage taken.");
            return;
        }

        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Player health: " + health);

        if (isLocalPlayer && healthBar != null)
        {
            healthBar.value = health;
        }

        if (health <= 0)
        {
            Debug.Log("Player died!");
            Die();
        }
    }

    void Die()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(RespawnAfterDelay(5f));
            PhotonNetwork.Destroy(gameObject);
        }
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RoomManager.Instance.SpawnPlayer();
    }
}
