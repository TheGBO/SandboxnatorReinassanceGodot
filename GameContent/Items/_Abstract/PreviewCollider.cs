using Godot;
using System;

public partial class PreviewCollider : Area3D
{
    public bool IsColliding => GetOverlappingBodies().Count > 0;
}
