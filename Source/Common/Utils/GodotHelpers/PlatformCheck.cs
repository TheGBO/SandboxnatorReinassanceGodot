using Godot;
namespace NullCyan.Util.GodotHelpers;

public static class PlatformCheck
{
    public static string OsName => OS.GetName().ToLower();

    public static bool IsDesktop()
    {
        return OsName == "windows" || OsName == "macos" || OsName == "linux" || OsName == "freebsd" || OsName == "bsd" || OsName == "openbsd";
    }

    public static bool IsMobile()
    {
        return OsName == "android" || OsName == "ios";
    }

    public static bool IsExport()
    {
        return OS.HasFeature("standalone");
    }
}