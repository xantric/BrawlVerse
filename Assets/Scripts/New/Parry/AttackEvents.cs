using System;
using UnityEngine;

public static class AttackEvents
{
    public static event Action<GameObject, GameObject> OnIncomingAttack;

    public static void Broadcast(GameObject attacker, GameObject target) 
    {
        Debug.Log("Attack Broadcast: " + attacker.name + "->" + target.name);
        OnIncomingAttack?.Invoke(attacker, target);
    }
}
