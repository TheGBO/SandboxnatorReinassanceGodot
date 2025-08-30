using Godot;
using System;

public partial class ENetTransport : Singleton<ENetTransport>, ITransport
{
	private ENetMultiplayerPeer peer;

	public int LocalPeerId => peer?.GetUniqueId() ?? -1;

	public bool IsServer => Multiplayer.IsServer();
	public bool IsNetConnected => peer?.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected;

	public event Action<long, byte[]> OnDataReceived;
	public event Action<long> OnPeerConnected;
	public event Action<long> OnPeerDisconnected;

	public override void _EnterTree()
	{
		SetInstance();
		SetActions();
	}

	public override void _Process(double delta)
	{
		if (peer == null) return;

		while (peer.GetAvailablePacketCount() > 0)
		{
			byte[] data = peer.GetPacket();

			if (data == null || data.Length == 0) continue;

			int senderId = peer.GetPacketPeer();

			OnDataReceived?.Invoke(senderId, data);
		}
	}


	private void SetActions()
	{
		Multiplayer.PeerConnected += id =>
		{
			OnPeerConnected?.Invoke(id);
		};

		Multiplayer.PeerDisconnected += id =>
		{
			OnPeerDisconnected?.Invoke(id);
		};
	}

	public void StartServer(int port)
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateServer(port);
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.ConnectedToServer += () => GD.Print("Connected to server!");
	}

	public void StartClient(string address, int port)
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);
		Multiplayer.MultiplayerPeer = peer;
	}

	public void Send(long peerId, byte[] data, bool reliable = true)
	{
		var mPeer = Multiplayer.MultiplayerPeer;
		peer.SetTargetPeer((int)peerId);
		peer.TransferMode = reliable ? MultiplayerPeer.TransferModeEnum.Reliable : MultiplayerPeer.TransferModeEnum.Unreliable;
		peer.PutPacket(data);
	}

	public void Broadcast(byte[] data, bool reliable = true)
	{
		var mPeer = Multiplayer.MultiplayerPeer;
		if (!Multiplayer.IsServer())
		{
			GD.PrintErr("[FATAL] Broadcast attempt on non-server peer");
			return;
		}
		peer.TransferMode = reliable ? MultiplayerPeer.TransferModeEnum.Reliable : MultiplayerPeer.TransferModeEnum.Unreliable;
		peer.SetTargetPeer((int)MultiplayerPeer.TargetPeerBroadcast);
		mPeer.PutPacket(data);
	}

	public void Stop()
	{
		peer?.Close();
		Multiplayer.MultiplayerPeer = null;
	}

}
