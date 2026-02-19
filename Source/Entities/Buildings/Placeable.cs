using Godot;
using NullCyan.Util.ComponentSystem;
using System;
namespace NullCyan.Sandboxnator.Building;

public partial class Placeable : RigidBody3D
{
    //TODO: destroy animation
    [Export] public ComponentHolder componentHolder;
    [Export] public AudioStreamPlayer3D placedSound;

    public void Destroy()
    {
        QueueFree();
    }

    public override void _Ready()
    {
        placedSound.Play();
    }
}
