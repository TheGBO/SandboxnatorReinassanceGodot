using Godot;
using System;

public partial class Snapper : Node3D
{
    public override void _EnterTree()
    {
        World.Instance.snappers.Add(this);
    }

    public override void _ExitTree()
    {
        World.Instance.snappers.Remove(this);
    }
}
