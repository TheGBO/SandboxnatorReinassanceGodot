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
    #region Controls

    [ExportCategory("Controls")]
    [SettingsSlider(SettingsCategory.Controls, "FOV", 32, 128, 1)]
    [Export] public double FieldOfView { get; set; } = 75;

    [SettingsSlider(SettingsCategory.Controls, "Sensitivity", 16, 256, 1)]
    [Export] public double LookSensitivity { get; set; } = 100;

    #endregion


    #region Graphics

    [ExportCategory("Graphics")]
    [SettingsToggle(SettingsCategory.Graphics, "Full screen")]
    [Export] public bool FullScreen { get; set; } = false;

    [SettingsToggle(SettingsCategory.Graphics, "Anti-aliasing")]
    [Export] public bool AntiAliasing { get; set; } = true;

    [SettingsToggle(SettingsCategory.Graphics, "VSync")]
    [Export] public bool VSync { get; set; } = true;

    [SettingsToggle(SettingsCategory.Graphics, "Show debug UI")]
    [Export] public bool DebugUiEnabled { get; set; } = false;

    [SettingsSlider(SettingsCategory.Graphics, "Rendering quality", 0.1f, 1.0f, 0.05f, "P0")]
    [Export] public float RenderScale { get; set; } = 1.0f;

    [SettingsToggle(SettingsCategory.Graphics, "Ambient Occlusion")]
    [Export] public bool AmbientOcclusion { get; set; } = true;

    [SettingsToggle(SettingsCategory.Graphics, "SSR")]
    [Export] public bool SSR { get; set; } = true;

    [SettingsToggle(SettingsCategory.Graphics, "SSIL")]
    [Export] public bool SSIL { get; set; } = true;

    [SettingsToggle(SettingsCategory.Graphics, "Fog (the fog is coming)")]
    [Export] public bool Fog { get; set; } = false;

    [SettingsSlider(SettingsCategory.Graphics, "Glow bloom", 0, 1, 0.01, "P0")]
    [Export] public float Bloom { get; set; } = 0.0f;

    #endregion
}