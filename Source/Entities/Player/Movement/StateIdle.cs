using NullGarel.Util.StateMachine;
namespace NullGarel.Sandboxnator.Entity;

public class StateIdle : IState<PlayerMovementContext>
{
    public IState<PlayerMovementContext> CheckTransitions(PlayerMovementContext ctx)
        => PlayerMovementTransitions.ResolveGroundedTransition(ctx);

    public void PhysicsProcess(PlayerMovementContext ctx, double delta)
        => ctx.HaltHorizontalMovement();
}