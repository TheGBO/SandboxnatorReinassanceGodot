using Godot;
using System;
namespace NullCyan.Util;

public partial class SaveLoader : Singleton<SaveLoader>
{
    public string SavePath => SetupGameSavePath();

    public override void _Ready()
    {
        SetupGameSavePath();
        GD.PrintRich($"[color=RED]{SavePath}[/color]");
    }

    public string SetupGameSavePath()
    {
        string pathRoot = "";
        if (PlatformCheck.IsDesktop())
        {
            pathRoot = OS.GetExecutablePath();
        }
        else
        {
            pathRoot = OS.GetUserDataDir();
        }
        return $"{pathRoot}/sandboxnator{ProjectSettings.GetSetting("application/config/version").ToString().Replace(".","_")}/";
    }
}
