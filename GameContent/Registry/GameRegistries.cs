using Godot;
using System;
using NullCyan.Sandboxnator.Commands;
using NullCyan.Sandboxnator.Item;
using NullCyan.Sandboxnator.Settings;
using NullCyan.Util;
using NullCyan.Util.Log;
using NullCyan.Util.GodotHelpers;
using NullCyan.Sandboxnator.Entity.PlayerCosmetics;
using NullCyan.Util.IO;
namespace NullCyan.Sandboxnator.Registry;

/// <summary>
/// Monolithic class for game-wide data
/// </summary>
public partial class GameRegistries : Singleton<GameRegistries>
{
    //DATA SECTION
    public Registry<ItemData> ItemRegistry { get; set; } = new();
    public Registry<PlayerFaceData> PlayerFaceRegistry { get; set; } = new();
    public Registry<ChatCommand> CommandRegistry { get; set; } = new();
    public Registry<PackedScene> BuildingRegistry { get; set; } = new();
    public GameSettingsData SettingsData { get; set; } = new();
    public string UserSettingsName { get; private set; } = "UserSettings.tres";

    [Export]
    public Texture2D BuildingPallete;

    public static string GetGameVersion => ProjectSettings.GetSetting("application/config/version").ToString();

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-EVENT BUS SECTION
    /// <summary>
    /// Called when settings are saved and SettingsData is reassigned.
    /// </summary>
    public Action OnSettingsSaved { get; set; }

    public override void _Ready()
    {
        NcLogger.Log("GAME REGISTRIES INITIALIZED", NcLogger.LogType.Register);
        try
        {
            LoadUserSettings();
        }
        catch (System.IO.FileNotFoundException)
        {
            //To avoid null reference exceptions.
            LoadDefaultSettings();
        }
        InitializeRegistries();
    }

    private void LoadDefaultSettings()
    {
        var settings = GD.Load<GameSettingsData>("res://GameContent/DefaultSettings.tres");
        if (settings == null)
        {
            NcLogger.Log("Failed to load default settings! Creating fallback.", NcLogger.LogType.Error);
            SettingsData = new();
        }
        else
        {
            SettingsData = settings;
        }
        OnSettingsSaved?.Invoke();
    }

    private void LoadUserSettings()
    {
        var settings = SaveLoader.Instance.ReadResource<GameSettingsData>(SaveFolder.Config, UserSettingsName);
        if (settings == null)
        {
            NcLogger.Error("User settings do not exist yet");
            throw new System.IO.FileNotFoundException();
        }
        NcLogger.Info("User settings do in fact exist.");
        SettingsData = settings;
        OnSettingsSaved?.Invoke();
    }

    private void InitializeRegistries()
    {
        ItemRegistryManager itemRegistryManager = new();
        itemRegistryManager.Register();
        CommandRegistryManager commandRegistryManager = new();
        commandRegistryManager.Register();
        PlayerFaceRegistryManager playerFaceRegistryManager = new();
        playerFaceRegistryManager.Register();
    }
}