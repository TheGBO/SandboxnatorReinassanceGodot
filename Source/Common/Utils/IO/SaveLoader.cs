using Godot;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.Log;
using NullCyan.Util.GodotHelpers;
using System;
namespace NullCyan.Util.IO;

public partial class SaveLoader : Singleton<SaveLoader>
{
    public string SavePath => SetupGameSavePath();
    public string LogSavePath { get; private set; }

    public override void _Ready()
    {
        CreateDirectoryIfNotExists(SetupGameSavePath());
        // setup once
        LogSavePath = SetupLogSavePath();
        GD.PrintRich($"[color=RED]{SavePath}[/color]");
    }

    public void SaveToLog(string msg)
    {
        if (string.IsNullOrEmpty(LogSavePath)) return;

        FileAccess file;

        // Check if file exists first
        if (FileAccess.FileExists(LogSavePath))
        {
            // Open existing file for reading and writing
            file = FileAccess.Open(LogSavePath, FileAccess.ModeFlags.ReadWrite);
            if (file != null)
            {
                file.SeekEnd();
            }
        }
        else
        {
            // Create new file
            file = FileAccess.Open(LogSavePath, FileAccess.ModeFlags.Write);
        }

        if (file != null)
        {
            using (file)
            {
                file.StoreString($"{msg}\n");
                GD.Print($"Log saved to: {LogSavePath}");
            }
        }
        else
        {
            GD.PrintErr("Failed to open file for writing");
        }
    }


    public string SetupGameSavePath()
    {
        string pathRoot = "";
        if (PlatformCheck.IsDesktop())
        {
            pathRoot = OS.GetExecutablePath().GetBaseDir();
        }
        else
        {
            pathRoot = OS.GetUserDataDir();
        }
        return $"{pathRoot}/sandboxnator{GameRegistries.Instance.GetGameVersion.Replace(".", "_")}";
    }

    public void CreateDirectoryIfNotExists(string path)
    {
        if (!DirAccess.DirExistsAbsolute(path))
        {
            Error error = DirAccess.MakeDirRecursiveAbsolute(path);
            if (error != Error.Ok)
            {
                NcLogger.Log($"FAILED TO CREATE FOLDER ON {path}", NcLogger.LogType.Error, NcLogger.LogFlags.UseDateTime);
            }
            else
            {
                NcLogger.Log($"Folder on {path} created :3", NcLogger.LogType.Info, NcLogger.LogFlags.UseDateTime);
            }
        }
        else
        {
            NcLogger.Log($"{path} creation denied as it already exists.", NcLogger.LogType.Info, NcLogger.LogFlags.UseDateTime);
        }
    }

    private string SetupLogSavePath()
    {
        //Initialize the logger with the game version and datetime ONCE.
        string logFileName = $"{DateTime.Now:yyyyMMddHHmmss}.sbxlog.txt";
        //TODO: make folders for each type of file, like a separate folder just for logs.
        string logSavePath = $"{SavePath}/{logFileName}";

        return logSavePath;
    }
}
