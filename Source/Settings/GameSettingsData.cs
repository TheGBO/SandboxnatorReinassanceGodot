using Godot;

namespace NullGarel.Sandboxnator.Settings;

/// <summary>
/// [DTO] Game settings such as FOV;
/// Convention: DTOs that are not sent over network should be resources
/// </summary>
[GlobalClass]
[GodotClassName("GameSettingsData")]
public partial class GameSettingsData : Resource
{
    [ExportCategory("Controls")]
    [SettingsSlider(SettingsCategory.Controls, "FOV", 32, 128, 1)]
    [Export] public double FieldOfView { get; set; } = 75;

    [SettingsSlider(SettingsCategory.Controls, "Sensitivity", 16, 256, 1)]
    [Export] public double LookSensitivity { get; set; } = 100;


    [ExportCategory("Graphics")]
    [SettingsToggle(SettingsCategory.Graphics, "Ambient Occlusion")]
    [Export] public bool AmbientOcclusion { get; set; } = true;

}