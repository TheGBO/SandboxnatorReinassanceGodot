using Godot;
using System;
namespace NullCyan.Sandboxnator.Item;

public partial class PreviewCollider : Area3D
{
    public bool IsColliding => GetOverlappingBodies().Count > 0;
}
