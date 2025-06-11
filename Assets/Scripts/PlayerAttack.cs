using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static event Action<Vector3, Vector3, float> PushAttack;

    Vector3 playerPosition;
    Vector3 attackDirection;
    [SerializeField] float attackRange = 3f;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("0");
            AttackInitiated();
            playerPosition = transform.position;
            attackDirection = transform.forward;
            StartCoroutine(DoDamageCoroutine(playerPosition,attackDirection,attackRange));
            
        }
    }

    IEnumerator DoDamageCoroutine(Vector3 playerPosition, Vector3 attackDirection, float attackRange) {
        yield return new WaitForSeconds(0.5f);
        DoDamage(playerPosition, attackDirection, attackRange); //Use Animation Events for precise Positioning
    }

    private void AttackInitiated() {
        ParryManager.OpenParryWindow?.Invoke();
    }

    public void DoDamage(Vector3 playerPosition, Vector3 attackDirection, float attackRange) {
        ParryManager.CloseParryWindow?.Invoke();
        PushAttack?.Invoke(playerPosition, attackDirection, attackRange);
    }
}
