using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private AttackData data;

    public PlayerAttackState(PlayerStateMachine ctx, PlayerStateFactory factory, AttackData attack)
        : base(ctx, factory)
    {
        data = attack;
    }

    public override void EnterState()
    {
        
        ctx.runtimeOverride["AttackBase"] = data.animation;
        ctx.animator.SetTrigger("isAttacking");
        ctx.animator.SetBool("IsAttacking", true);
    }

    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    {
        ctx.animator.SetBool("IsAttacking", false);
    }
    public void ApplyDamage()
    {
        if (!ctx.attackOriginMap.TryGetValue(data.AttackOriginName, out var origin))
        {
            Debug.LogWarning($"No attack origin found for '{data.AttackOriginName}'. Using player transform as fallback.");
            origin = ctx.transform;
        }

        Collider[] hits = Physics.OverlapSphere(origin.position, data.range, ctx.EnemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyHealth>(out var health))
                health.TakeDamage(data.damage);
        }
    }

    
}
