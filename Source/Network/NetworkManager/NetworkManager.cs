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

		#region Connection state tracking

		private double _connectionStartTime = 0.0;

		public const double ConnectionTimeoutLimit = 5.0;

		private bool _connectionInProgress = false;
		private bool _enetConnected = false;

		public bool IsConnecting => _connectionInProgress;

		public double ElapsedConnectionTime
		{
			get
			{
				if (!_connectionInProgress)
					return 0.0;

				return
					(Time.GetTicksMsec() / 1000.0) -
					_connectionStartTime;
			}
		}
		#endregion

		#region signal connection state
		private bool _serverSignalsConnected = false;
		private bool _clientSignalsConnected = false;
		#endregion

		#region application events
		public event Action ConnectionStarted;
		public event Action ConnectionEstablished;
		public event Action ConnectionFailed;
		public event Action TimedOut;
		#endregion

		#region Godot lifecycle
		public override void _Ready()
		{
			InitializeNetworkManager();
		}

		public override void _Process(double delta)
		{
			if (!_connectionInProgress)
				return;

			if (peer == null)
			{
				FailConnection(
					"Connection peer disappeared while connecting.",
					false);

				return;
			}

			double elapsed = ElapsedConnectionTime;

			if (elapsed >= ConnectionTimeoutLimit)
			{
				if (_enetConnected)
				{
					NcLogger.Log(
						$"[!] ENet connected, but Sandboxnator handshake timed out after {ConnectionTimeoutLimit} seconds.",
						NcLogger.LogType.Warn);
				}
				else
				{
					NcLogger.Log(
						$"[!] ENet connection timed out after {ConnectionTimeoutLimit} seconds.",
						NcLogger.LogType.Warn);
				}

				TimedOut?.Invoke();

				FailConnection(
					"Connection attempt timed out.",
					false);
			}
		}
		#endregion

		#region initialization
		private void InitializeNetworkManager()
		{
			NcLogger.Log(
				"Sandboxnator multiplayer protocol initialized");

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

		#endregion

		#region public state
		public bool HasMultiplayerPeer()
		{
			return Multiplayer != null &&
				   Multiplayer.HasMultiplayerPeer() &&
				   Multiplayer.MultiplayerPeer != null;
		}
		#endregion

		#region server
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

			SandboxnatorMain.Instance.LoadWorld();
			if (!dedicatedServer)
			{
				long hostId = Multiplayer.GetUniqueId();

				NcLogger.Log(
					$"[V] Adding host player with ID {hostId}.");

				PlayerManager.Instance.AddPlayer(hostId);
			}

			NcLogger.Log(
				"[V] Server started successfully.");
		}
		#endregion

		#region client connection handling
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

				ConnectionFailed?.Invoke();

				return;
			}

			peer = newPeer;
			Multiplayer.MultiplayerPeer = peer;

			ConnectClientSignals();

			_connectionStartTime =
				Time.GetTicksMsec() / 1000.0;

			_enetConnected = false;
			_connectionInProgress = true;

			NcLogger.Log(
				$"[V] Attempting to connect to {ip}:{port}...");

			ConnectionStarted?.Invoke();
		}

		private void OnConnectedToServer()
		{
			if (!_connectionInProgress)
			{
				NcLogger.Log(
					"[!] Received ConnectedToServer without an active connection attempt.",
					NcLogger.LogType.Warn);

				return;
			}

			_enetConnected = true;

			NcLogger.Log(
				"[V] ENet connection established.");

			NcLogger.Log(
				"[V] Sending Sandboxnator handshake...");

			PlayerManager.Instance.SendHandShake();
		}

		//sandboxnator level connection handshake
		public void NotifyConnectionEstablished()
		{
			if (!_connectionInProgress)
			{
				NcLogger.Log(
					"[!] Received handshake acknowledgement when no connection was pending.",
					NcLogger.LogType.Warn);

				return;
			}

			_connectionInProgress = false;

			NcLogger.Log(
				"[V] Sandboxnator connection established.");

			ConnectionEstablished?.Invoke();
		}

		private void OnConnectionFailed()
		{
			if (!_connectionInProgress)
				return;

			FailConnection(
				"ENet failed to establish a connection.",
				false);
		}

		private void FailConnection(
			string reason,
			bool invokeTimeoutEvent)
		{
			if (!_connectionInProgress)
				return;

			_connectionInProgress = false;

			NcLogger.Log(
				$"[X] {reason}",
				NcLogger.LogType.Warn);

			if (invokeTimeoutEvent)
				TimedOut?.Invoke();

			ConnectionFailed?.Invoke();

			QuitConnection();
		}

		public async void QuitConnection()
		{
			_connectionInProgress = false;
			_enetConnected = false;

			if (Multiplayer.MultiplayerPeer == null)
			{
				peer = null;
				return;
			}

			NcLogger.Log("[!] Closing multiplayer connection...");

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
				PlayerManager.Instance.PrepareForDisconnect();
				Multiplayer.MultiplayerPeer = null;
			}

			NcLogger.Log(
				"[V] Multiplayer connection fully closed.");
		}
		#endregion

		#region server signals
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

		#endregion

		#region client signals
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
		#endregion

		#region signal cleanup
		private void DisconnectAllSignals()
		{
			DisconnectServerSignals();
			DisconnectClientSignals();
		}
		#endregion

		#region peer cleanup
		private void CleanupOldPeer()
		{
			_connectionInProgress = false;
			_enetConnected = false;

			DisconnectAllSignals();

			if (Multiplayer.MultiplayerPeer == null)
			{
				peer = null;
				return;
			}

			NcLogger.Log(
				"[!] Cleaning up old multiplayer session before creating a new one...",
				NcLogger.LogType.Warn);

			MultiplayerPeer oldPeer =
				Multiplayer.MultiplayerPeer;


			PlayerManager.Instance?.PrepareForDisconnect();
			oldPeer.Close();
			Multiplayer.MultiplayerPeer = null;

			peer = null;
		}
	}
		#endregion
}