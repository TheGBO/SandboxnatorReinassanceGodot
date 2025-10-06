using System;
using Godot;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util.IO;
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
    [Export] private double broadcastTimeout = 5;
    private double _elapsedTime = 0;
    public Action<ServerInfoData> OnBroadcastReceived;

    public override void _Ready()
    {
        SetupListen();
    }

    public override void _Process(double delta)
    {
        _elapsedTime += delta;
        if (_elapsedTime < broadcastTimeout)
            return;

        _elapsedTime = 0;

        var networkPeer = NetworkManager.Instance?.peer;
        if (networkPeer != null && Multiplayer.IsServer())
        {
            BroadcastServerInfo();
        }

        if (_listener.GetAvailablePacketCount() > 0)
        {
            byte[] serverInfoBytes = _listener.GetPacket();
            //Needs to happen on receiver side in order to get proper broadcasting and correct IP addr
            string serverIP = _listener.GetPacketIP();
            int serverPort = _listener.GetPacketPort();
            ServerInfoData serverInfo = MPacker.Unpack<ServerInfoData>(serverInfoBytes);
            serverInfo.IpAddress = serverIP;
            serverInfo.Port = serverPort;
            OnBroadcastReceived?.Invoke(serverInfo);
        }
    }

    public void SetupListen()
    {
        var ok = _listener.Bind(broadcastListenPort);
        if (ok == Error.Ok)
        {
            NcLogger.Log($"network discovery client started at {broadcastListenPort}");
        }
        else
        {
            NcLogger.Log($"FAILED TO INIT DISCOVERY CLIENT", NcLogger.LogType.Error);
        }
    }

    public void SetupBroadcast()
    {
        // only server can broadcast, obviously...
        if (!Multiplayer.IsServer()) return;

        _broadcaster.SetBroadcastEnabled(true);
        _broadcaster.SetDestAddress(broadcastAddress, broadcastListenPort);
        var ok = _broadcaster.Bind(broadcastHostPort);
        if (ok == Error.Ok)
        {
            NcLogger.Log(
                $"Broadcast service successfully initialized at {broadcastHostPort} targetting The address of {broadcastAddress}:{broadcastListenPort}.",
                NcLogger.LogType.Info,
                NcLogger.LogFlags.UseDateTime | NcLogger.LogFlags.ShouldSave
            );
        }
    }

    private void BroadcastServerInfo()
    {
        GD.Print("Broadcasting server!");
        ServerInfoData serverInfo = new()
        {
            Name = PlayerProfileManager.Instance.CurrentProfile.PlayerName,
            GameVersion = GameRegistries.GetGameVersion
        };

        _broadcaster.PutPacket(MPacker.Pack(serverInfo));
    }
}