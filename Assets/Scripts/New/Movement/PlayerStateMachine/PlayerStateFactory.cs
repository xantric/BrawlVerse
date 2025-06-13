using UnityEngine;

public class PlayerStateFactory
{
    private PlayerStateMachine ctx;

    public PlayerStateFactory(PlayerStateMachine context)
    {
        ctx = context;
    }

    public PlayerBaseState Idle() => new PlayerIdleState(ctx, this);
    //public PlayerBaseState Walk() => new PlayerWalkState(ctx, this);
    public PlayerBaseState Run() => new PlayerRunState(ctx, this);
    public PlayerBaseState Jump() => new PlayerJumpState(ctx, this);
}
