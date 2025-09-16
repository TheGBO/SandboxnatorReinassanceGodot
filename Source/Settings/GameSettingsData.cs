using Godot;

namespace NullCyan.Sandboxnator.Settings;

/// <summary>
/// [DTO] Game settings such as FOV;
/// Convention: DTOs that are not sent over network should be resources
/// </summary>
[GlobalClass]
[GodotClassName("GameSettingsData")]
public partial class GameSettingsData : Resource
{
    [ExportCategory("Controls")]
    [Export] public float Sensitivity { get; set; } = 100;
    [Export] public float FieldOfView { get; set; } = 75;
    //[ExportCategory("Graphics")]
}