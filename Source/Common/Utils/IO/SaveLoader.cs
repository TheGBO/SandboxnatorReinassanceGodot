using Godot;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.Log;
using NullCyan.Util.GodotHelpers;
using System;
namespace NullCyan.Util.IO;

/// <summary>
/// A monolithic file IO class.
/// </summary>
public partial class SaveLoader : Singleton<SaveLoader>
{
    public string SavePath => SetupGameSavePath();

    public override void _Ready()
    {
        // Ensure root save directory exists
        CreateDirectoryIfNotExists(SavePath);

        // Pre-create all known subfolders
        foreach (SaveFolder folder in Enum.GetValues(typeof(SaveFolder)))
        {
            GetFolderPath(folder);
        }

        NcLogger.Log($"Save path initialized at: {SavePath}", NcLogger.LogType.Info);
    }

    #region Path helpers

    /// <summary>
    /// Setup the path where the game data will be saved
    /// </summary>
    /// <returns>returns the selected path as a string</returns>
    private string SetupGameSavePath()
    {
        //the base directory
        string pathRoot = (PlatformCheck.IsDesktop() && PlatformCheck.IsExport())
            ? OS.GetExecutablePath().GetBaseDir()
            : OS.GetUserDataDir();

        //the complete directory. (game version sensitive)
        return $"{pathRoot}/sandboxnator_{GameRegistries.GetGameVersion.Replace(".", "_")}";
    }

    /// <summary>
    /// Utility that returns the path based on a specified enum.
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public string GetFolderPath(SaveFolder folder)
    {
        string subDir = folder switch
        {
            SaveFolder.Logs => "logs",
            SaveFolder.PlayerProfiles => "profiles",
            SaveFolder.Worlds => "worlds",
            SaveFolder.Config => "config",
            SaveFolder.Temp => "temp",
            SaveFolder.Misc => "misc",
            _ => "misc"
        };

        string fullPath = $"{SavePath}/{subDir}";
        CreateDirectoryIfNotExists(fullPath);
        return fullPath;
    }

    //self explanatory name
    public void CreateDirectoryIfNotExists(string path)
    {
        if (!DirAccess.DirExistsAbsolute(path))
        {
            Error error = DirAccess.MakeDirRecursiveAbsolute(path);
            if (error != Error.Ok)
            {
                NcLogger.Log($"FAILED TO CREATE FOLDER: {path}", NcLogger.LogType.Error, NcLogger.LogFlags.UseDateTime);
            }
            else
            {
                NcLogger.Log($"Folder created: {path}", NcLogger.LogType.Info, NcLogger.LogFlags.UseDateTime);
            }
        }
    }
    #endregion

    #region Generic file helpers

    /// <summary>
    /// append==true and the file exists, open without clearing and append.
    /// append==true and the file does NOT exist => create file and write.
    /// append==false, overwrite.
    /// Includes fallback attempts and logs FileAccess.GetOpenError() when open fails.
    /// </summary>
    public string WriteTextFile(SaveFolder folder, string fileName, string content, bool append = false)
    {
        string folderPath = GetFolderPath(folder);
        string path = $"{folderPath}/{fileName}";

        try
        {
            FileAccess file = null;

            if (append)
            {
                if (FileAccess.FileExists(path))
                {
                    file = FileAccess.Open(path, FileAccess.ModeFlags.ReadWrite);
                }
                else
                {
                    file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
                }
            }
            else
            {
                file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead);
            }

            if (file == null)
            {
                Error openErr = FileAccess.GetOpenError();
                NcLogger.Log($"Initial open failed for {path} (err {openErr}). Trying fallback open (WRITE).",
                    NcLogger.LogType.Warn, NcLogger.LogFlags.UseDateTime);

                file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
                if (file == null)
                {
                    Error openErr2 = FileAccess.GetOpenError();
                    NcLogger.Log($"Fallback open also failed for {path} (err {openErr2}).",
                        NcLogger.LogType.Error, NcLogger.LogFlags.UseDateTime);
                    return string.Empty;
                }
            }

            using (file)
            {
                if (append)
                {
                    file.SeekEnd();
                }
                file.StoreString(content);
                file.Flush();
            }

            return path;
        }
        catch (Exception ex)
        {
            NcLogger.Log($"Exception writing file {path}: {ex.Message}", NcLogger.LogType.Error, NcLogger.LogFlags.UseDateTime);
            GD.PrintErr($"[SaveLoader] Exception writing file {path}: {ex}");
            return string.Empty;
        }
    }

    public string ReadTextFile(SaveFolder folder, string fileName)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";
        if (!FileAccess.FileExists(path)) return string.Empty;

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        return file?.GetAsText() ?? string.Empty;
    }

    public bool DeleteFile(SaveFolder folder, string fileName)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";
        return FileAccess.FileExists(path) && DirAccess.RemoveAbsolute(path) == Error.Ok;
    }

    public void WriteBytes(SaveFolder folder, string fileName, byte[] data)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file?.StoreBuffer(data);
    }

    public byte[] ReadBytes(SaveFolder folder, string fileName)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";
        if (!FileAccess.FileExists(path)) return null;
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        return file?.GetBuffer((long)file.GetLength());
    }
    #endregion

    #region Specialized helpers

    public void SaveToLog(string msg)
    {
        string fileName = $"{DateTime.Now:yyyyMMdd-HH:mm:ss}.log".Replace(":", "_");
        WriteTextFile(SaveFolder.Logs, fileName, $"{DateTime.Now:u} {msg}\n", append: true);
    }

    #endregion
}
