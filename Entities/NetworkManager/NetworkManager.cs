using Godot;
using System;


//todo: make proper network manager
public partial class NetworkManager : Node3D
{
	ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
	[Export] PackedScene playerScene;

    public override void _EnterTree()
    {
        GD.Print("Sandboxnator protocol initialized");
    }

    private void HostGame(int port = 1077){
		peer.CreateServer(port);	
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += AddPlayer;
		AddPlayer(1);
	}

	private void JoinGame(int port = 1077, string ip = "localhost"){
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;
	}

	private void AddPlayer(long id = 1){
		var player = playerScene.Instantiate();
		player.Name = id.ToString();
		CallDeferred("add_child", player);
	}


}
