using Godot;
using NullGarel.Util.ComponentSystem;
using System;
namespace NullGarel.Sandboxnator.Building;

public partial class Placeable : RigidBody3D
{
    //TODO: destroy animation
    [Export] public ComponentHolder componentHolder;

    public void Destroy()
    {
        QueueFree();
    }

}
