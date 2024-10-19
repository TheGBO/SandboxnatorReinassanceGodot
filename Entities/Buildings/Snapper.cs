using Godot;
using System;

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
        if (body is Building)
        {
            InsideBody = true;
            GD.Print("I am inside a building, therefore I shall deactivate");
        }
    }

    public void _on_area_3d_body_exited(Node3D body)
    {
        if (body is Building)
        {
            InsideBody = false;
            GD.Print("I am outside a building, therefore I shall activate");
        }
    }
}
