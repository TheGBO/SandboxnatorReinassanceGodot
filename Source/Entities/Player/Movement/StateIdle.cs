using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.StateMachine;
using System;
namespace NullGarel.Sandboxnator.Entity;

public class StateIdle : IState<PlayerMovementContext>
{
    public IState<PlayerMovementContext> CheckTransitions(PlayerMovementContext ctx)
        => PlayerMovementTransitions.ResolveGroundedTransition(ctx);

    public void PhysicsProcess(PlayerMovementContext ctx, double delta)
        => ctx.HaltHorizontalMovement();
}