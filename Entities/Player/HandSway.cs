using Godot;
using System;

public partial class HandSway : AbstractPlayerComponent
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

        if (parent.playerInput.LookVector.X > swayThreshold)
        {
            hand.Rotation = hand.Rotation.Lerp(rightSway, swaySpeed * (float)delta);
        }
        else if (parent.playerInput.LookVector.X < -swayThreshold)
        {
            hand.Rotation = hand.Rotation.Lerp(leftSway, swaySpeed * (float)delta);
        }
        else
        {
            hand.Rotation = hand.Rotation.Lerp(Vector3.Zero, swaySpeed * (float)delta);
        }
    }
}
