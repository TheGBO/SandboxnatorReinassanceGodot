using System;
using Godot;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util.IO;
using NullCyan.Util.Log;

namespace NullCyan.Sandboxnator.Network;

//TODO: This doesn't work and it pisses me off lol.
/// <summary>
/// An in-game background service that is responsible for LAN matchmaking/server discovery
/// This class in specific is the back-end of the server browser in the UI.
/// </summary>
[GodotClassName("ServerDiscovery")]
public partial class ServerDiscovery : Singleton<ServerDiscovery>
{
    private PacketPeerUdp _broadcaster = new();
    private PacketPeerUdp _listener = new();
    // The broadcast service host runs on this port.
    [Export] private int broadcastHostPort = 49201;
    // The target is this port, since the host doesn't care much (255.255.255.255), it is slightly randomized to avoid collisions when testing on the same machine.
    [Export] private int broadcastListenPort = 49202;
    // I think this is some sort of magic number to send packets to everyone.
    [Export] private string broadcastAddress = "255.255.255.255";
    // Timeout is a bit misleading, maybe broadcastDelay would be a better name
    [Export] private double broadcastTimeout = 5;
    // keep track of time //TODO: maybe move this to a singleton called "ClientTimeTracker"
    private double _elapsedTime = 0;
    public Action<ServerInfoData> OnBroadcastReceived;

    public override void _Ready()
    {
        //SetupListen();
    }

    public override void _Process(double delta)
    {
        return;

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
        var ok = _listener.Bind(0);
        if (ok == Error.Ok)
        {
            NcLogger.Log($"network discovery client started on random port");
        }
        else
        {
            NcLogger.Log($"FAILED TO INIT DISCOVERY CLIENT", NcLogger.LogType.Error);
        }
    }

    // This basically makes the server aivalable in order to broadcast
    public void SetupBroadcast()
    {
        if (!Multiplayer.IsServer()) return;

        _broadcaster.SetBroadcastEnabled(true);
        //servers broadcast to the *shared* listening port
        _broadcaster.SetDestAddress(broadcastAddress, broadcastListenPort);

        //random free local port to make the operating system stop bitching aroond
        var ok = _broadcaster.Bind(0);

        if (ok == Error.Ok)
        {
            NcLogger.Log(
                $"Broadcast service initialized (random bind) targeting {broadcastAddress}:{broadcastListenPort}",
                NcLogger.LogType.Info,
                NcLogger.LogFlags.UseDateTime | NcLogger.LogFlags.ShouldSave
            );
        }
    }

    // make the server stop it.
    // Violent language is funny so i'll keep kill instead of shutdown xDDDD edgy wowz!!!!
    public void KillBroadcast()
    {
        if (!Multiplayer.IsServer()) return;
        _broadcaster.SetBroadcastEnabled(false);
        // End the broadcasting.
        _broadcaster.Close();
    }

    // The host runs this and shoves the packet onto the broadcast thingy
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