using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.StateMachine;
using System;
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