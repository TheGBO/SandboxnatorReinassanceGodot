using System;
using System.Reflection.Metadata;
using Godot;

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
        UseDateTime,
        ShouldSave,
    }

    public const LogFlags DEFAULT_LOG_FLAGS = LogFlags.ShouldSave | LogFlags.UseDateTime;

    public static void Log(string msg, LogType logType = LogType.Info, LogFlags logFlags = DEFAULT_LOG_FLAGS)
    {
        switch (logType)
        {
            case LogType.Error:
                Log(msg, logFlags);
                break;
            case LogType.Info:
                LogInfo(msg, logFlags);
                break;
            case LogType.Register:
                LogRegister(msg, logFlags);
                break;
            case LogType.Warn:
                LogWarn(msg, logFlags);
                break;
        }
    }

    public static void LogWithColor(string msg, Color color, string header = "Basic Log", LogFlags logFlags = DEFAULT_LOG_FLAGS)
    {
        GD.PrintRich($"[color={color.ToHtml()}]{header}[/color] :: {msg}");
        if (logFlags.HasFlag(LogFlags.ShouldSave))
            SaveLoader.Instance.SaveToLog($"({header}) :: {msg}");
    }

    private static void Log(string msg, LogFlags logFlags)
    {
        LogWithColor(msg, Colors.Red, "[ERROR]", logFlags);
    }

    private static void LogInfo(string msg, LogFlags logFlags)
    {
        LogWithColor(msg, Colors.Green, "[INFO]", logFlags);
    }

    private static void LogRegister(string msg, LogFlags logFlags)
    {
        LogWithColor(msg, Colors.Blue, "[REGISTER]", logFlags);
    }

    private static void LogWarn(string msg, LogFlags logFlags)
    {
        LogWithColor(msg, Colors.Yellow, "[WARN]", logFlags);
    }
}