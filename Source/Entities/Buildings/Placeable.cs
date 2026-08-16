using Godot;
using NullGarel.Util.ComponentSystem;
using System;
using System.Linq;
namespace NullGarel.Sandboxnator.Building;

public partial class Placeable : RigidBody3D
{
    //TODO: destroy animation
    [Export] public ComponentHolder componentHolder;
    private bool _hasInteractable;
    public bool HasInteractable => _hasInteractable;

    public override void _Ready()
    {
        var interactable = componentHolder
            .GetChildren()
            .OfType<IInteractable>()
            .FirstOrDefault();
        _hasInteractable = interactable != null;
    }

    public void Destroy()
    {
        QueueFree();
    }

}
