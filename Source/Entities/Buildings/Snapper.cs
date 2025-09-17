using Godot;
using System;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util;
namespace NullCyan.Sandboxnator.Building;

public partial class Snapper : Node3D
{
    public bool InsideBody { get; private set; }

    public override void _EnterTree()
    {
        World.Instance.snappers.Add(this);
    }

    public override void _ExitTree()
    {
        World.Instance.snappers.Remove(this);
    }

    public void _on_area_3d_body_entered(Node3D body)
    {
        if (body is Placeable)
        {
            InsideBody = true;
        }
    }

    public void _on_area_3d_body_exited(Node3D body)
    {
        if (body is Placeable)
        {
            InsideBody = false;
        }
    }
}
