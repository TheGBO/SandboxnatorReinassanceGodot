using Godot;
using System;
using NullCyan.Sandboxnator.Commands;
using NullCyan.Sandboxnator.Item;
using NullCyan.Sandboxnator.Settings;
using NullCyan.Util;
namespace NullCyan.Sandboxnator.Registry;

/// <summary>
/// Monolithic class for game-wide data
/// </summary>
public partial class GameRegistries : Singleton<GameRegistries>
{
    //DATA SECTION
    public Registry<ItemData> ItemRegistry { get; set; } = new();
    public Registry<ChatCommand> CommandRegistry { get; set; } = new();
    public Registry<PackedScene> BuildingRegistry { get; set; } = new();
    public GameSettingsData SettingsData { get; set; } = new();

    //EVENT BUS SECTION
    /// <summary>
    /// Called when settings are saved and SettingsData is reassigned.
    /// </summary>
    public Action OnSettingsSaved { get; set; }

    public override void _Ready()
    {
        GD.PrintRich("[color=cyan]GAME REGISTRIES INITIALIZED[/color]");
        //To avoid null reference exceptions.
        LoadDefaultSettings();
        InitializeRegistries();
    }

    private void LoadDefaultSettings()
    {
        var settings = GD.Load<GameSettingsData>("res://GameContent/DefaultSettings.tres");
        if (settings == null)
        {
            GD.PrintErr("Failed to load default settings! Creating fallback.");
            SettingsData = new GameSettingsData();
        }
        else
        {
            SettingsData = settings;
        }
        OnSettingsSaved?.Invoke();
    }

    private void InitializeRegistries()
    {
        ItemRegistryManager itemRegistryManager = new();
        itemRegistryManager.Register();
        CommandRegistryManager commandRegistryManager = new();
        commandRegistryManager.Register();
    }
}