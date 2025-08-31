using Godot;
using System;
using System.Linq;


public partial class NetworkManager : Singleton<NetworkManager>
{
	public ENetMultiplayerPeer peer = new ENetMultiplayerPeer();

	public override void _Ready()
	{
		Multiplayer.PeerDisconnected += PlayerManager.Instance.LogOutPlayer;
		GD.Print("Sandboxnator multiplayer protocol initialized");
		string[] args = OS.GetCmdlineArgs();
		bool dedicatedServer = args.Contains("server") && !args.Contains("client");
		GD.Print($"Dedicated server check-up: {dedicatedServer}");
		if (dedicatedServer)
		{
			//TODO: make port configurable by arguments
			HostGame(1077, true);
		}
	}

	public void HostGame(int port = 1077, bool dedicatedServer = false)
	{
		peer.CreateServer(port);
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += PlayerManager.Instance.AddPlayer;
		PlayerManager.Instance.AddPlayer(1);
	}

	public void JoinGame(int port = 1077, string ip = "127.0.0.1")
	{
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;
	}

}
