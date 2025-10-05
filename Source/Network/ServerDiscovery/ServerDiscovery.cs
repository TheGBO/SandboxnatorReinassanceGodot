using Godot;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util.Log;

namespace NullCyan.Sandboxnator.Network;

/// <summary>
/// An in-game background service that is responsible for LAN matchmaking/server discovery
/// </summary>
[GodotClassName("ServerDiscovery")]
public partial class ServerDiscovery : Singleton<ServerDiscovery>
{
    private PacketPeerUdp _broadcaster = new();
    private PacketPeerUdp _listener = new();
    [Export] private int broadcastHostPort = 49201;
    [Export] private int broadcastListenPort = 49202;
    [Export] private string broadcastAddress = "255.255.255.255";
    [Export] private Timer broadcastTimer;

    public override void _Ready()
    {
        broadcastTimer.Timeout += SetupBroadcast;
    }

    public void SetupBroadcast()
    {
        // only server can broadcast, obviously...
        if (!Multiplayer.IsServer()) return;
        ServerInfo serverInfo = new()
        {
            Name = PlayerProfileManager.Instance.CurrentProfile.PlayerName,
            GameVersion = GameRegistries.GetGameVersion
        };
        _broadcaster.SetBroadcastEnabled(true);
        _broadcaster.SetDestAddress(broadcastAddress, broadcastListenPort);
        var ok = _broadcaster.Bind(broadcastHostPort);
        if (ok == Error.Ok)
        {
            NcLogger.Log(
                @$"Broadcast service successfully initialized at {broadcastHostPort} targetting
                The address of {broadcastAddress}:{broadcastListenPort}.
                ",
                NcLogger.LogType.Info,
                NcLogger.LogFlags.UseDateTime | NcLogger.LogFlags.ShouldSave
            );
        }
    }
}