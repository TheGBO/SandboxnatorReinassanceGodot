using Godot;
using System;
namespace NullCyan.Sandboxnator.Building;

public partial class Placeable : RigidBody3D
{
    //TODO: destroy animation
    public void Destroy()
    {
        QueueFree();
    }
}
