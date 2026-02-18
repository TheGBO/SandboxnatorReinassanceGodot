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
    [Export] public double FieldOfView { get; set; } = 75;
    [Export] public double LookSensitivity { get; set; } = 100;
    [ExportCategory("Graphics")]
    [Export] public bool AmbientOcclusion = false;
}