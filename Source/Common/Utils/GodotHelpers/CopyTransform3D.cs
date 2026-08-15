using Godot;
using Godot.Collections;

namespace NullGarel.Util.GodotHelpers;
/// <summary>
/// Used when it is not practical to reparent a node just to inherit rotation.
/// </summary>
[Tool]
[GlobalClass]
public partial class CopyTransform3D : Node3D
{
    [ExportCategory("Common")]
    [Export] private Node3D _reference;
    [ExportCategory("Rotation")]
    [Export] private Array<Node3D> _rotateAlongReference;
    [ExportCategory("Position")]
    [Export] private Array<Node3D> _moveAlongReference;
    [Export] private Vector3 _posOffset;

    public override void _Process(double dt)
    {
        foreach (Node3D item in _rotateAlongReference)
        {
            if (item == null) continue;
            item.GlobalRotation = _reference.GlobalRotation;
        }
        foreach (Node3D item in _moveAlongReference)
        {
            if (item == null) continue;
            item.GlobalPosition = _reference.GlobalPosition + _posOffset;
        }
    }
}