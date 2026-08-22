using Godot;
namespace NullGarel.Sandboxnator.Item;

public partial class PreviewCollider : Area3D
{
    public bool IsColliding => GetOverlappingBodies().Count > 0;
}
