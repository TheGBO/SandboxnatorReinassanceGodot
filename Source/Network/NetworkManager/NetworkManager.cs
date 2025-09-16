using Godot;
using System;
using System.Linq;
using NullCyan.Util;
using NullCyan.Sandboxnator.WorldAndScenes;

namespace NullCyan.Sandboxnator.Network
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		public ENetMultiplayerPeer peer;

		// Timeout handling
		private float connectionStartTime = 0f;
		private float connectionTimeout = 5f;
		private bool waitingForConnection = false;

		public override void _Ready()
		{
			GD.Print("Sandboxnator multiplayer protocol initialized");

			// Dedicated server boot check
			string[] args = OS.GetCmdlineArgs();
			bool dedicatedServer = args.Contains("server") && !args.Contains("client");
			GD.Print($"Dedicated server check-up: {dedicatedServer}");

			if (dedicatedServer)
			{
				HostGame(1077, true);
			}
		}

		public override void _Process(double delta)
		{
			if (waitingForConnection && peer != null)
			{
				float elapsed = (Time.GetTicksMsec() / 1000f) - connectionStartTime;

				if (elapsed >= connectionTimeout &&
					peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting)
				{
					GD.PrintErr($"⏳ Connection timed out after {connectionTimeout} seconds.");
					OnConnectionFailed();
				}
			}
		}
		
		public bool HasMultiplayerPeer()
		{
			return Multiplayer != null &&
				Multiplayer.HasMultiplayerPeer() &&
				Multiplayer.MultiplayerPeer != null;
		}

		/// <summary>
		/// Initialize server-side game.
		/// </summary>
		public void HostGame(int port = 1077, bool dedicatedServer = false)
		{
			CleanupOldPeer();
			GD.Print($"✅ Hosting server on port {port} | Dedicated: {dedicatedServer}");

			peer = new ENetMultiplayerPeer();
			Error result = peer.CreateServer(port);

			if (result != Error.Ok)
			{
				GD.PrintErr($"❌ Could not create server at the specified port: {port}");
				return;
			}

			Multiplayer.MultiplayerPeer = peer;

			// Hook server-side signals
			//TODO: Validate client game version, do NOT allow players with versions different from that of the server.
			Multiplayer.PeerDisconnected += PlayerManager.Instance.RemovePlayer;
			Multiplayer.PeerConnected += PlayerManager.Instance.AddPlayer;

			// Add host player manually, dedicated servers have no player, hosts are servers with a player.
			if (!dedicatedServer)
				PlayerManager.Instance.AddPlayer(Multiplayer.GetUniqueId());

		}

		/// <summary>
		/// Initialize client-side game.
		/// </summary>
		public void JoinGame(int port = 1077, string ip = "127.0.0.1")
		{
			CleanupOldPeer();

			peer = new ENetMultiplayerPeer();
			Error result = peer.CreateClient(ip, port);

			if (result != Error.Ok)
			{
				GD.PrintErr($"❌ Failed to start client: {result}");
				return;
			}

			Multiplayer.MultiplayerPeer = peer;

			// Hook client-specific signals
			Multiplayer.ConnectedToServer += OnConnectedToServer;
			Multiplayer.ConnectionFailed += OnConnectionFailed;

			GD.Print($"🔌 Attempting to connect to {ip}:{port}...");

			// Start timeout tracking
			connectionStartTime = Time.GetTicksMsec() / 1000f;
			waitingForConnection = true;
		}

		/// <summary>
		/// Disconnects from server or shuts down hosted server safely.
		/// </summary>
		/// //TODO: FIX DISCONNECTION
		public async void QuitConnection()
		{
			if (Multiplayer.MultiplayerPeer == null)
				return;

			GD.Print("🔻 Closing multiplayer connection...");

			// Disconnect signals to avoid unwanted callbacks during shutdown
			if (Multiplayer.IsServer())
			{
				//Server signals
				Multiplayer.PeerDisconnected -= PlayerManager.Instance.RemovePlayer;
				Multiplayer.PeerConnected -= PlayerManager.Instance.AddPlayer;
			}
			else
			{
				//Client signals
				Multiplayer.ConnectedToServer -= OnConnectedToServer;
				Multiplayer.ConnectionFailed -= OnConnectionFailed;
			}

			// Tell ENet to close the session
			if (peer != null && peer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Disconnected)
			{
				peer.Close();

				// Wait until the peer status changes to Disconnected
				while (peer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Disconnected)
				{
					await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
				}
			}

			// Remove the MultiplayerPeer reference
			Multiplayer.MultiplayerPeer = null;
			peer = null;
			waitingForConnection = false;
			//TODO: Create a NullCyan.Util logger class so i can make log files.
			GD.Print("✅ Multiplayer connection fully closed.");
		}



		private void OnConnectedToServer()
		{
			waitingForConnection = false;
			GD.Print("✅ Successfully connected to server!");
		}

		private void OnConnectionFailed()
		{
			waitingForConnection = false;
			GD.PrintErr("❌ Failed to connect to server. It may not exist or be unreachable.");
			QuitConnection();
		}

		/// <summary>
		/// Ensures any previous multiplayer peer is properly closed before creating a new one.
		/// </summary>
		private void CleanupOldPeer()
		{
			if (Multiplayer.MultiplayerPeer != null)
			{
				GD.Print("⚠️ Cleaning up old multiplayer session before creating a new one...");
				Multiplayer.MultiplayerPeer.Close();
				Multiplayer.MultiplayerPeer = null;
				peer = null;
				waitingForConnection = false;
			}
		}
	}
}
