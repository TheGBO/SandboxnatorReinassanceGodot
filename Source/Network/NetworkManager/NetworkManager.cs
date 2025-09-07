using Godot;
using System;
using System.Linq;
using NullCyan.Util;
using NullCyan.Sandboxnator.WorldAndScenes;
namespace NullCyan.Sandboxnator.Network;

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


	/// <summary>
	/// Initialize server-side game.
	/// </summary>
	/// <param name="port">network port</param>
	/// <param name="dedicatedServer">//TODO: not fully implemented yet</param>
	public void HostGame(int port = 1077, bool dedicatedServer = false)
	{
		try
		{
			peer.CreateServer(port);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Could not create server at port due to error: {e.Message}");
			return;
		}
		if (peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected)
		{
			Multiplayer.MultiplayerPeer = peer;
			Multiplayer.PeerConnected += PlayerManager.Instance.AddPlayer;
			PlayerManager.Instance.AddPlayer(1);
			
		}
	}


	/// <summary>
	/// Initialize client-side game.
	/// </summary>
	/// <param name="port">network port</param>
	/// <param name="ip">ipv4 address</param>
	public void JoinGame(int port = 1077, string ip = "127.0.0.1")
	{
		try
		{
			peer.CreateClient(ip, port);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Could not create client at port due to error: {e.Message}");
			return;
		}
		Multiplayer.MultiplayerPeer = peer;
	}

}
