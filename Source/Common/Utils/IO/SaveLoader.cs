using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util.GodotHelpers;
using NullGarel.Util.Log;
using System;

namespace NullGarel.Util.IO;

/// <summary>
/// Provides utilities for reading and writing game save data.
/// </summary>
public static class SaveLoader
{
    public static string SavePath => SetupGameSavePath();


    #region Path helpers

    /// <summary>
    /// Gets the path where the game data will be saved.
    /// </summary>
    private static string SetupGameSavePath()
    {
        string pathRoot =
            (PlatformCheck.IsDesktop() && PlatformCheck.IsExport())
                ? OS.GetExecutablePath().GetBaseDir()
                : OS.GetUserDataDir();

        return $"{pathRoot}/sandboxnator_{GameRegistries.GetGameVersion.Replace(".", "_")}";
    }


    /// <summary>
    /// Returns the path for a specified save folder.
    /// The folder is created if it does not already exist.
    /// </summary>
    public static string GetFolderPath(SaveFolder folder)
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


    /// <summary>
    /// Creates a directory and all required parent directories if they do not exist.
    /// </summary>
    public static void CreateDirectoryIfNotExists(string path)
    {
        if (DirAccess.DirExistsAbsolute(path))
            return;

        Error error = DirAccess.MakeDirRecursiveAbsolute(path);

        if (error != Error.Ok)
        {
            NcLogger.Log(
                $"FAILED TO CREATE FOLDER: {path}",
                NcLogger.LogType.Error,
                NcLogger.LogFlags.UseDateTime);
        }
        else
        {
            NcLogger.Log(
                $"Folder created: {path}",
                NcLogger.LogType.Info,
                NcLogger.LogFlags.UseDateTime);
        }
    }

    #endregion


    #region Generic file helpers

    /// <summary>
    /// Writes text to a file.
    /// If append is true, existing contents are preserved and the new content
    /// is written at the end of the file.
    /// </summary>
    public static string WriteTextFile(
        SaveFolder folder,
        string fileName,
        string content,
        bool append = false)
    {
        string folderPath = GetFolderPath(folder);
        string path = $"{folderPath}/{fileName}";

        try
        {
            FileAccess file;

            if (append)
            {
                file = FileAccess.FileExists(path)
                    ? FileAccess.Open(path, FileAccess.ModeFlags.ReadWrite)
                    : FileAccess.Open(path, FileAccess.ModeFlags.Write);
            }
            else
            {
                file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead);
            }

            if (file == null)
            {
                Error openErr = FileAccess.GetOpenError();

                NcLogger.Log(
                    $"Initial open failed for {path} (err {openErr}). Trying fallback open (WRITE).",
                    NcLogger.LogType.Warn,
                    NcLogger.LogFlags.UseDateTime);

                file = FileAccess.Open(path, FileAccess.ModeFlags.Write);

                if (file == null)
                {
                    Error fallbackError = FileAccess.GetOpenError();

                    NcLogger.Log(
                        $"Fallback open also failed for {path} (err {fallbackError}).",
                        NcLogger.LogType.Error,
                        NcLogger.LogFlags.UseDateTime);

                    return string.Empty;
                }
            }

            using (file)
            {
                if (append)
                    file.SeekEnd();

                file.StoreString(content);
                file.Flush();
            }

            return path;
        }
        catch (Exception ex)
        {
            NcLogger.Log(
                $"Exception writing file {path}: {ex.Message}",
                NcLogger.LogType.Error,
                NcLogger.LogFlags.UseDateTime);

            GD.PrintErr($"[SaveLoader] Exception writing file {path}: {ex}");

            return string.Empty;
        }
    }


    public static string ReadTextFile(
        SaveFolder folder,
        string fileName)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";

        if (!FileAccess.FileExists(path))
            return string.Empty;

        using var file = FileAccess.Open(
            path,
            FileAccess.ModeFlags.Read);

        return file?.GetAsText() ?? string.Empty;
    }


    public static bool DeleteFile(
        SaveFolder folder,
        string fileName)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";

        return FileAccess.FileExists(path)
            && DirAccess.RemoveAbsolute(path) == Error.Ok;
    }


    public static void WriteBytes(
        SaveFolder folder,
        string fileName,
        byte[] data)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";

        using var file = FileAccess.Open(
            path,
            FileAccess.ModeFlags.Write);

        file?.StoreBuffer(data);
    }


    public static byte[] ReadBytes(
        SaveFolder folder,
        string fileName)
    {
        string path = $"{GetFolderPath(folder)}/{fileName}";

        if (!FileAccess.FileExists(path))
            return null;

        using var file = FileAccess.Open(
            path,
            FileAccess.ModeFlags.Read);

        return file?.GetBuffer((long)file.GetLength());
    }

    #endregion


    #region Specialized helpers

    public static void SaveToLog(string msg)
    {
        string fileName =
            $"{DateTime.Now:yyyyMMdd-HH:mm:ss}.log"
                .Replace(":", "_");

        WriteTextFile(
            SaveFolder.Logs,
            fileName,
            $"{DateTime.Now:u} {msg}\n",
            append: true);
    }

    #endregion


    #region Resource helpers

    public static Error WriteResource(
        SaveFolder folder,
        string fileName,
        Resource resource)
    {
        if (!fileName.EndsWith(".tres"))
            fileName += ".tres";

        string path = $"{GetFolderPath(folder)}/{fileName}";

        Error error = ResourceSaver.Save(resource, path);

        if (error != Error.Ok)
        {
            NcLogger.Log(
                $"Failed to save resource to {path}: {error}",
                NcLogger.LogType.Error);
        }

        return error;
    }


    public static T ReadResource<T>(
        SaveFolder folder,
        string fileName)
        where T : Resource
    {
        if (!fileName.EndsWith(".tres"))
            fileName += ".tres";

        string path = $"{GetFolderPath(folder)}/{fileName}";

        if (!FileAccess.FileExists(path))
            return null;

        return ResourceLoader.Load<T>(path);
    }

    #endregion
}