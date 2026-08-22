using Godot;
namespace NullGarel.Sandboxnator.Entity;

[GlobalClass]
public partial class PlayerMovementStats : Resource
{
    [Export] public float WalkSpeed { get; private set; } = 3.75f;
    [Export] public float SprintSpeed { get; private set; } = 6.25f;
    [Export] public float JumpVelocity { get; private set; } = 8.5f;
}
