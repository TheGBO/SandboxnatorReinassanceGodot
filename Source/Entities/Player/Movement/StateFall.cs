using NullGarel.Util.StateMachine;
namespace NullGarel.Sandboxnator.Entity;

public class StateFall : IState<PlayerMovementContext>
{
    public IState<PlayerMovementContext> CheckTransitions(PlayerMovementContext ctx)
    {
        if (ctx.CharacterBody.IsOnFloor())
        {
            //transition to whatever when touching the ground
            return PlayerMovementTransitions.ResolveGroundedTransition(ctx);
        }
        return null; //continue falling
    }

    public void PhysicsProcess(PlayerMovementContext ctx, double delta)
    {
        ctx.ProcessHorizontalMovement(delta);
        ctx.ProcessGravity(delta);
    }
}