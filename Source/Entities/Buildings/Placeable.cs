using Godot;
using System;

public partial class Placeable : RigidBody3D
{
    //TODO: destroy animation
    public void Destroy()
    {
        QueueFree();
    }
}
