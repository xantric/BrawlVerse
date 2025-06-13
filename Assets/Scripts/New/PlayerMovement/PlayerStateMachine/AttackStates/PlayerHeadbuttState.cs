//using UnityEngine;

//public class PlayerHeadbuttState : PlayerAttackState
//{

//    public PlayerHeadbuttState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory) { }

//    public override void EnterState()
//    {
//        ctx.animator.SetBool("isHeadbutting", true);

//    }

//    public override void UpdateState()
//    {
        
//    }

//    public override void ExitState()
//    {
//        ctx.animator.SetBool("isHeadbutting", false);
//    }

//    public void ApplyHeadbuttDamage()
//    {
//        Collider[] hitEnemies = Physics.OverlapSphere(ctx.HeadbuttAttackOrigin.position, ctx.HeadbuttAttackRange, ctx.EnemyLayer);

//        foreach (Collider enemy in hitEnemies)
//        {
//            if (enemy.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
//            {
//                enemyHealth.TakeDamage(ctx.HeadbuttDamage);
//            }
//        }
//    }
//    public void EndAttack()
//    {
//        ctx.SwitchState(factory.Idle());
//    }


//}
