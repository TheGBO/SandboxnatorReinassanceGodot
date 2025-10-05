using Godot;
using System;
using System.Linq;
using NullCyan.Util;
using NullCyan.Util.GodotHelpers;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util.Log;

namespace NullCyan.Sandboxnator.Network
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		public ENetMultiplayerPeer peer;

		// Timeout handling
		private float _connectionStartTime = 0f;
		private float _connectionTimeout = 5f;
		private bool _waitingForConnection = false;

		public override void _Ready()
		{
			NcLogger.Log("Sandboxnator multiplayer protocol initialized");

			// Dedicated server boot check
			string[] args = OS.GetCmdlineArgs();
			bool dedicatedServer = args.Contains("server") && !args.Contains("client");
			NcLogger.Log($"Dedicated server check-up: {dedicatedServer}");

			if (dedicatedServer)
			{
				HostGame(1077, true);
			}
		}

		public override void _Process(double delta)
		{
			if (_waitingForConnection && peer != null)
			{
				float elapsed = (Time.GetTicksMsec() / 1000f) - _connectionStartTime;

				if (elapsed >= _connectionTimeout &&
					peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting)
				{
					NcLogger.Log($"[!] Connection timed out after {_connectionTimeout} seconds.", NcLogger.LogType.Warn);
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
			NcLogger.Log($"[V] Hosting server on port {port} | Dedicated: {dedicatedServer}");

			peer = new();
			Error result = peer.CreateServer(port);

			if (result != Error.Ok)
			{
				NcLogger.Log($"[X] Could not create server at the specified port: {port}");
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

			ServerDiscovery.Instance.SetupBroadcast();
		}

		/// <summary>
		/// Initialize client-side game.
		/// </summary>
		public void JoinGame(int port = 1077, string ip = "127.0.0.1")
		{
			CleanupOldPeer();

			peer = new();
			Error result = peer.CreateClient(ip, port);

			if (result != Error.Ok)
			{
				NcLogger.Log($"❌ Failed to start client: {result}");
				return;
			}

			Multiplayer.MultiplayerPeer = peer;

			// Hook client-specific signals
			Multiplayer.ConnectedToServer += OnConnectedToServer;
			Multiplayer.ConnectionFailed += OnConnectionFailed;

			NcLogger.Log($"🔌 Attempting to connect to {ip}:{port}...");

			// Start timeout tracking
			_connectionStartTime = Time.GetTicksMsec() / 1000f;
			_waitingForConnection = true;
		}

		/// <summary>
		/// Disconnects from server or shuts down hosted server safely.
		/// </summary>
		public async void QuitConnection()
		{
			if (Multiplayer.MultiplayerPeer == null)
				return;

			NcLogger.Log("[!] Closing multiplayer connection...");

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
			_waitingForConnection = false;
			NcLogger.Log("[V] Multiplayer connection fully closed.");
		}



		private void OnConnectedToServer()
		{
			_waitingForConnection = false;
			NcLogger.Log("[V] Successfully connected to server!");
		}

		private void OnConnectionFailed()
		{
			_waitingForConnection = false;
			NcLogger.Log("[X] Failed to connect to server. It may not exist or be unreachable.");
			QuitConnection();
		}

		/// <summary>
		/// Ensures any previous multiplayer peer is properly closed before creating a new one.
		/// </summary>
		private void CleanupOldPeer()
		{
			if (Multiplayer.MultiplayerPeer != null)
			{
				NcLogger.Log("[!] Cleaning up old multiplayer session before creating a new one...", NcLogger.LogType.Warn);
				Multiplayer.MultiplayerPeer.Close();
				Multiplayer.MultiplayerPeer = null;
				peer = null;
				_waitingForConnection = false;
			}
		}
	}
}
