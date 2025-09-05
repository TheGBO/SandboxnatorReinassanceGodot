using Godot;
using System;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;

public partial class HandSway : AbstractComponent<Player>
{
    [Export] private float swaySpeed;
    [Export] private Node3D hand;
    [Export] private Vector3 rightSway;
    [Export] private Vector3 leftSway;
    [Export] private float swayThreshold;

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
    }



    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;

        if (ComponentParent.playerInput.LookVector.X > swayThreshold)
        {
            hand.Rotation = hand.Rotation.Lerp(rightSway, swaySpeed * (float)delta);
        }
        else if (ComponentParent.playerInput.LookVector.X < -swayThreshold)
        {
            hand.Rotation = hand.Rotation.Lerp(leftSway, swaySpeed * (float)delta);
        }
        else
        {
            hand.Rotation = hand.Rotation.Lerp(Vector3.Zero, swaySpeed * (float)delta);
        }
    }
}
