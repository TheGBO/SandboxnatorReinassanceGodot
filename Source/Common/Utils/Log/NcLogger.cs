using System;
using Godot;
using NullCyan.Util.IO;
namespace NullCyan.Util.Log;

/// <summary>
/// NullCyan logger
/// </summary>
public static class NcLogger
{
    public enum LogType
    {
        Error,
        Info,
        Register,
        Warn
    }

    [Flags]
    public enum LogFlags
    {
        UseDateTime = 1,
        ShouldSave = 2,
    }

    public const LogFlags DEFAULT_LOG_FLAGS = LogFlags.ShouldSave | LogFlags.UseDateTime;

    // Map log types to their default color and header
    private static readonly (Color Color, string Header)[] LogTypeMap =
    {
        (Colors.Red, "[ERROR]"),
        (Colors.Green, "[INFO]"),
        (Colors.Cyan, "[REGISTER]"),
        (Colors.Yellow, "[WARN]")
    };

    //aliases
    public static void Info(string msg, LogFlags flags = DEFAULT_LOG_FLAGS)
    => Log(msg, LogType.Info, flags);

    public static void Warn(string msg, LogFlags flags = DEFAULT_LOG_FLAGS)
        => Log(msg, LogType.Warn, flags);

    public static void Error(string msg, LogFlags flags = DEFAULT_LOG_FLAGS)
        => Log(msg, LogType.Error, flags);

    public static void Register(string msg, LogFlags flags = DEFAULT_LOG_FLAGS)
        => Log(msg, LogType.Register, flags);


    /// <summary>
    /// General log function
    /// </summary>
    public static void Log(string msg, LogType logType = LogType.Info, LogFlags logFlags = DEFAULT_LOG_FLAGS)
    {
        var (color, header) = LogTypeMap[(int)logType];

        if (logFlags.HasFlag(LogFlags.UseDateTime))
            header = $"[{DateTime.Now:HH:mm:ss}] {header}";

        GD.PrintRich($"[color={color.ToHtml()}]{header}[/color] :: {msg}");

        if (logFlags.HasFlag(LogFlags.ShouldSave))
            SaveLoader.Instance.SaveToLog($"({header}) :: {msg}");
    }

    /// <summary>
    /// Flexible logging with custom color and header
    /// </summary>
    public static void LogWithColor(string msg, Color color, string header = "Basic Log", LogFlags logFlags = DEFAULT_LOG_FLAGS)
    {
        if (logFlags.HasFlag(LogFlags.UseDateTime))
            header = $"[{DateTime.Now:HH:mm:ss}] {header}";

        GD.PrintRich($"[color={color.ToHtml()}]{header}[/color] :: {msg}");

        if (logFlags.HasFlag(LogFlags.ShouldSave))
            SaveLoader.Instance.SaveToLog($"({header}) :: {msg}");
    }
}
