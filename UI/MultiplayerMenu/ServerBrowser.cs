using Godot;
using NullGarel.Sandboxnator.Network;
using NullGarel.Util.Log;

namespace NullGarel.Sandboxnator.UI;

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
