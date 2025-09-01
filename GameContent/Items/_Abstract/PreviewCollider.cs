using Godot;
using System;

public partial class PreviewCollider : Area3D
{
    public bool IsColliding { get; private set; }

    public override void _Ready()
    {
        BodyEntered += _on_body_entered;
        BodyExited += _on_body_exited;
    }

    public void _on_body_entered(Node3D body)
    {
        IsColliding = true;
    }
    
    public void _on_body_exited(Node3D body)
    {
        IsColliding = false;
    }
}
