using Godot;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.Log;
using System;
namespace NullCyan.Util;

public partial class SaveLoader : Singleton<SaveLoader>
{
    public string SavePath => SetupGameSavePath();
    public string LogSavePath { get; private set; }

    public override void _Ready()
    {
        SetupGameSavePath();
        // setup once
        LogSavePath = SetupLogSavePath();
        GD.PrintRich($"[color=RED]{SavePath}[/color]");
    }

    public void SaveToLog(string msg)
    {
        if (string.IsNullOrEmpty(LogSavePath)) return;

        using var file = FileAccess.Open(LogSavePath, FileAccess.ModeFlags.Write);
        file.StoreString($"{msg}");
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
        return $"{pathRoot}/sandboxnator{GameRegistries.Instance.GetGameVersion.Replace(".", "_")}/";
    }

    private string SetupLogSavePath()
    {
        //Initialize the logger with the game version and datetime ONCE.
        string logFileName = $"{DateTime.Now:yyyyMMddHHmmss}.sbxlog.txt";
        string logSavePath = $"{SavePath}/{logFileName}";

        return logSavePath;
    }
}
