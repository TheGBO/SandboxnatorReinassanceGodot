using Godot;
using System;
using System.Linq;
using NullGarel.Util;
using NullGarel.Util.GodotHelpers;
using NullGarel.Sandboxnator.WorldAndScenes;
using NullGarel.Util.Log;

namespace NullGarel.Sandboxnator.Network
{
	public partial class NetworkManager : Singleton<NetworkManager>
	{
		public ENetMultiplayerPeer peer;

		private float _connectionStartTime = 0f;
		[Export]
		public float ConnectionTimeoutLimit { get; set; } = 32f;
		private float _elapsed;
		public float ElapsedConnectionTime => _elapsed;
		private bool _waitingForConnection = false;
		private bool _waitingForHandshake = false;
		public bool IsConnecting => _waitingForConnection || _waitingForHandshake;

		private bool _serverSignalsConnected = false;
		private bool _clientSignalsConnected = false;

		public event Action ConnectionStarted;
		public event Action ConnectionEstablished;
		public event Action ConnectionFailed;
		public event Action TimedOut;

		public override void _Ready()
		{
			InitializeNetworkManager();
		}

		private void InitializeNetworkManager()
		{
			NcLogger.Log("Sandboxnator multiplayer protocol initialized");

			string[] args = OS.GetCmdlineArgs();

			bool dedicatedServer =
				args.Contains("server") &&
				!args.Contains("client");

			NcLogger.Log(
				$"Dedicated server check-up: {dedicatedServer}");

			if (dedicatedServer)
			{
				HostGame(1077, true);
			}
		}

		public override void _Process(double delta)
		{
			if (!_waitingForConnection || peer == null)
				return;

			_elapsed =
				(Time.GetTicksMsec() / 1000f) -
				_connectionStartTime;

			if (_elapsed >= ConnectionTimeoutLimit &&
				peer.GetConnectionStatus() ==
				MultiplayerPeer.ConnectionStatus.Connecting)
			{
				NcLogger.Log(
					$"[!] Connection timed out after {ConnectionTimeoutLimit} seconds.",
					NcLogger.LogType.Warn);

				OnConnectionFailed();
				TimedOut?.Invoke();
			}
		}

		public bool HasMultiplayerPeer()
		{
			return Multiplayer != null &&
				Multiplayer.HasMultiplayerPeer() &&
				Multiplayer.MultiplayerPeer != null;
		}

		/// <summary>
		/// Starts a server.
		/// </summary>
		public void HostGame(
			int port = 1077,
			bool dedicatedServer = false)
		{
			CleanupOldPeer();

			NcLogger.Log(
				$"[V] Hosting server on port {port} | Dedicated: {dedicatedServer}");

			ENetMultiplayerPeer newPeer = new();

			Error result = newPeer.CreateServer(port);

			if (result != Error.Ok)
			{
				NcLogger.Log(
					$"[X] Could not create server at port {port}: {result}",
					NcLogger.LogType.Error);

				return;
			}

			peer = newPeer;
			Multiplayer.MultiplayerPeer = peer;

			ConnectServerSignals();

			if (!dedicatedServer)
			{
				long hostId = Multiplayer.GetUniqueId();

				NcLogger.Log(
					$"[V] Adding host player with ID {hostId}.");

				PlayerManager.Instance.AddPlayer(hostId);
			}
		}

		/// <summary>
		/// Starts a client and connects to a server.
		/// </summary>
		public void JoinGame(
			int port = 1077,
			string ip = "127.0.0.1")
		{
			CleanupOldPeer();

			NcLogger.Log(
				$"[V] Connecting to {ip}:{port}...");

			ENetMultiplayerPeer newPeer = new();

			Error result = newPeer.CreateClient(ip, port);

			if (result != Error.Ok)
			{
				NcLogger.Log(
					$"[X] Failed to create client: {result}",
					NcLogger.LogType.Error);

				return;
			}

			peer = newPeer;
			Multiplayer.MultiplayerPeer = peer;

			ConnectClientSignals();

			_connectionStartTime =
				Time.GetTicksMsec() / 1000f;

			ConnectionStarted?.Invoke();

			_waitingForConnection = true;

			NcLogger.Log(
				$"[V] Attempting to connect to {ip}:{port}...");
		}

		/// <summary>
		/// Safely closes the current multiplayer connection.
		/// </summary>
		public async void QuitConnection()
		{
			if (Multiplayer.MultiplayerPeer == null)
			{
				_waitingForConnection = false;
				peer = null;
				return;
			}

			NcLogger.Log(
				"[!] Closing multiplayer connection...");

			_waitingForConnection = false;

			DisconnectAllSignals();

			ENetMultiplayerPeer oldPeer = peer;

			peer = null;

			if (oldPeer != null &&
				oldPeer.GetConnectionStatus() !=
				MultiplayerPeer.ConnectionStatus.Disconnected)
			{
				oldPeer.Close();

				while (oldPeer.GetConnectionStatus() !=
					   MultiplayerPeer.ConnectionStatus.Disconnected)
				{
					await ToSignal(
						GetTree(),
						SceneTree.SignalName.ProcessFrame);
				}
			}

			if (Multiplayer.MultiplayerPeer == oldPeer)
			{
				Multiplayer.MultiplayerPeer = null;
			}

			NcLogger.Log(
				"[V] Multiplayer connection fully closed.");
		}

		private void OnConnectedToServer()
		{
			_waitingForHandshake = true;

			NcLogger.Log(
				"[V] Successfully connected to server!");


			PlayerManager.Instance.SendHandShake();
		}

		public void NotifyConnectionEstablished()
		{
			_waitingForHandshake = false;
			_waitingForConnection = false;
			ConnectionEstablished?.Invoke();
		}

		private void OnConnectionFailed()
		{
			if (!_waitingForConnection)
				return;

			_waitingForConnection = false;
			_waitingForHandshake = false;

			NcLogger.Log(
				"[X] Failed to connect to server. " +
				"It may not exist or be unreachable.",
				NcLogger.LogType.Warn);

			QuitConnection();
			ConnectionFailed?.Invoke();
		}

		private void ConnectServerSignals()
		{
			if (_serverSignalsConnected)
				return;

			if (PlayerManager.Instance == null)
			{
				NcLogger.Log(
					"[X] Cannot connect server signals: PlayerManager.Instance is null.",
					NcLogger.LogType.Error);

				return;
			}

			Multiplayer.PeerDisconnected +=
				PlayerManager.Instance.RemovePlayer;

			Multiplayer.PeerConnected +=
				PlayerManager.Instance.OnPeerConnected;

			_serverSignalsConnected = true;

			NcLogger.Log(
				"[V] Server multiplayer signals connected.");
		}

		private void DisconnectServerSignals()
		{
			if (!_serverSignalsConnected)
				return;

			if (PlayerManager.Instance != null)
			{
				Multiplayer.PeerDisconnected -=
					PlayerManager.Instance.RemovePlayer;

				Multiplayer.PeerConnected -=
					PlayerManager.Instance.OnPeerConnected;
			}

			_serverSignalsConnected = false;

			NcLogger.Log(
				"[V] Server multiplayer signals disconnected.");
		}

		private void ConnectClientSignals()
		{
			if (_clientSignalsConnected)
				return;

			Multiplayer.ConnectedToServer +=
				OnConnectedToServer;

			Multiplayer.ConnectionFailed +=
				OnConnectionFailed;

			_clientSignalsConnected = true;

			NcLogger.Log(
				"[V] Client multiplayer signals connected.");
		}

		private void DisconnectClientSignals()
		{
			if (!_clientSignalsConnected)
				return;

			Multiplayer.ConnectedToServer -=
				OnConnectedToServer;

			Multiplayer.ConnectionFailed -=
				OnConnectionFailed;

			_clientSignalsConnected = false;

			NcLogger.Log(
				"[V] Client multiplayer signals disconnected.");
		}

		private void DisconnectAllSignals()
		{
			DisconnectServerSignals();
			DisconnectClientSignals();
		}

		/// <summary>
		/// Cleans up an existing multiplayer session before
		/// creating a new server or client.
		/// </summary>
		private void CleanupOldPeer()
		{
			_waitingForConnection = false;

			DisconnectAllSignals();

			if (Multiplayer.MultiplayerPeer == null)
			{
				peer = null;
				return;
			}

			NcLogger.Log(
				"[!] Cleaning up old multiplayer session " +
				"before creating a new one...",
				NcLogger.LogType.Warn);

			MultiplayerPeer oldPeer =
				Multiplayer.MultiplayerPeer;

			oldPeer.Close();

			Multiplayer.MultiplayerPeer = null;

			peer = null;
		}
	}
}
