using Godot;
using MessagePack;
namespace NullCyan.Sandboxnator.Network;

[MessagePackObject]
public class ServerInfoData
{
    [Key(0)]
    public string Name { get; set; }
    [Key(1)]
    public string GameVersion { get; set; }
    [Key(2)]
    public string IpAddress { get; set; }
    [Key(3)]
    public int Port { get; set; }
    public override string ToString()
    {
        return $"server name:{Name}\ngame version:{GameVersion}\naddr.:{IpAddress}:{Port}";
    }
}