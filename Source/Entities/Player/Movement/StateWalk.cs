using NullGarel.Util.StateMachine;
namespace NullGarel.Sandboxnator.Entity;

public class StateWalk : IState<PlayerMovementContext>
{
    public void Enter(PlayerMovementContext ctx) => ctx.CurrentSpeed = ctx.WalkSpeed;

    public IState<PlayerMovementContext> CheckTransitions(PlayerMovementContext ctx)
        => PlayerMovementTransitions.ResolveGroundedTransition(ctx);

    public void PhysicsProcess(PlayerMovementContext ctx, double delta)
        => ctx.ProcessHorizontalMovement(delta);
}