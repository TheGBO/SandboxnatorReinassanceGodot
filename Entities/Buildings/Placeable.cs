using Godot;
using System;

public partial class Placeable : RigidBody3D
{
    public void Destroy()
    {
        QueueFree();
    }
}
