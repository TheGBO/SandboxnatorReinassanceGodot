using Godot;
using NullCyan.Sandboxnator.Network;
using NullCyan.Util.Log;
using System;

namespace NullCyan.Sandboxnator.UI;

public partial class ServerBrowser : Panel
{
    [Export] PackedScene serverInfoContainer;
    // 
    public override void _Ready()
    {
        ServerDiscovery.Instance.OnBroadcastReceived += HandleServerDiscovery;
    }

    private void HandleServerDiscovery(ServerInfoData serverInfoData)
    {
        NcLogger.Log("Received a server");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}
