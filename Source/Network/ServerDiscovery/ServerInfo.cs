using Godot;
using MessagePack;
namespace NullCyan.Sandboxnator.Network;

[MessagePackObject]
public class ServerInfo
{
    [Key(0)]
    public string Name { get; set; }
    [Key(1)]
    public string GameVersion { get; set; }

}