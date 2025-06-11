using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageReceiver : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] float attackPower = 2f;
    [SerializeField] float maxAngle = 60f;
    private void OnEnable() {
        ParrySupport_Attack.PushAttack += ReceivedPushAttack;
    }
    private void OnDisable() {
        ParrySupport_Attack.PushAttack -= ReceivedPushAttack;  
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void ReceivedPushAttack(Vector3 playerPosition, Vector3 attackDirection, float attackRange) {
        float distance = Vector3.Distance(playerPosition, transform.position);
        if (distance <= attackRange) {
            Vector3 directionFromPlayer = transform.position - playerPosition;
            directionFromPlayer.Normalize();
            if(Vector3.Dot(directionFromPlayer, attackDirection) >= Mathf.Cos(maxAngle * Mathf.Deg2Rad)) {
                rb.AddForce(attackDirection * attackPower, ForceMode.Impulse);
            }
        }
    }
}
