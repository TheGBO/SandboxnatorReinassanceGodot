using NullGarel.Util.StateMachine;
namespace NullGarel.Sandboxnator.Entity;

public class StateJump : IState<PlayerMovementContext>
{
    public IState<PlayerMovementContext> CheckTransitions(PlayerMovementContext ctx)
    {
        return new StateFall();
    }

    public void PhysicsProcess(PlayerMovementContext ctx, double delta)
    {
        ctx.ApplyJumpImpulse();
        ctx.ProcessHorizontalMovement(delta);
    }
}