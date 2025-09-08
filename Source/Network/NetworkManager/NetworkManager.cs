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
		private float connectionTimeout = 15f;
		private bool waitingForConnection = false;

		public override void _Ready()
		{
			GD.Print("Sandboxnator multiplayer protocol initialized");

			// Hook default signals
			Multiplayer.PeerDisconnected += PlayerManager.Instance.RemovePlayer;

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

		/// <summary>
		/// Initialize server-side game.
		/// </summary>
		public void HostGame(int port = 1077, bool dedicatedServer = false)
		{
			CleanupOldPeer();

			peer = new ENetMultiplayerPeer();
			Error result = peer.CreateServer(port);

			if (result != Error.Ok)
			{
				GD.PrintErr($"❌ Could not create server at the specified port: {port}");
				return;
			}

			Multiplayer.MultiplayerPeer = peer;

			// Hook server-side signals
			Multiplayer.PeerConnected += PlayerManager.Instance.AddPlayer;

			// Add host player manually
			PlayerManager.Instance.AddPlayer(Multiplayer.GetUniqueId());

			GD.Print($"✅ Hosting server on port {port} | Dedicated: {dedicatedServer}");
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
		public void QuitConnection()
		{
			if (Multiplayer.MultiplayerPeer == null)
				return;

			GD.Print("🔻 Closing multiplayer connection...");

			if (peer != null &&
				(peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected ||
				 peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting))
			{
				// Remove local player before closing
				PlayerManager.Instance.RemovePlayer(Multiplayer.GetUniqueId());
				peer.Close();
			}

			// Reset networking state
			Multiplayer.MultiplayerPeer = null;
			peer = null;
			waitingForConnection = false;

			// Unhook all events
			Multiplayer.ConnectedToServer -= OnConnectedToServer;
			Multiplayer.ConnectionFailed -= OnConnectionFailed;
			Multiplayer.PeerConnected -= PlayerManager.Instance.AddPlayer;
			Multiplayer.PeerDisconnected -= PlayerManager.Instance.RemovePlayer;
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
