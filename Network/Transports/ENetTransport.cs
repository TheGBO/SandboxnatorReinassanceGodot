using Godot;
using System;
using System.Text;

public partial class ENetTransport : Singleton<ENetTransport>
{
	private ENetMultiplayerPeer peer;

	public int LocalPeerId => peer?.GetUniqueId() ?? -1;

	public bool IsServer => Multiplayer.IsServer();
	public bool IsClient => throw new NotImplementedException();

	public bool IsNetConnected => peer?.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected;

	public event Action<long, byte[]> DataReceivedEvent;
	public event Action<long> PeerConnectedEvent;
	public event Action<long> PeerDisconnectedEvent;
	public event Action ClientConnectedToServer;

	public override void _EnterTree()
	{
		SetInstance();
		SetActions();
	}

	public override void _Process(double delta)
	{
		if (peer == null) return;
		GD.Print($"Connection status: {peer.GetConnectionStatus()}");
		if (!IsServer && peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting)
		{
			
		}
		
		
		// Poll for new packets (even empty ones to force the connection)
		while (peer.GetAvailablePacketCount() > 0)
		{
			byte[] data = peer.GetPacket();
			int senderId = peer.GetPacketPeer();
			GD.Print($"{data} from {senderId}");
			PeerConnectedEvent?.Invoke(senderId);
		}

		/*
		for (int i = 0; i < peer.GetPeerCount(); i++)
		{
			int id = peer.GetPeerId(i);
			if (!connectedPeers.Contains(id))
			{
				connectedPeers.Add(id);
				GD.Print($"New client connected: {id}");
			}
		}
		*/
	}


	private void SetActions()
	{
		Multiplayer.PeerConnected += id =>
		{
			PeerConnectedEvent?.Invoke(id);
		};

		Multiplayer.PeerDisconnected += id =>
		{
			PeerDisconnectedEvent?.Invoke(id);
		};
	}

	public void StartServer(int port)
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateServer(port);
		//Multiplayer.MultiplayerPeer = peer;
		Multiplayer.ConnectedToServer += () => GD.Print("Connected to server!");
		
	}

	public void StartClient(string address, int port)
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);
		//Multiplayer.MultiplayerPeer = peer;
	}

	public void Send(long peerId, byte[] data, bool reliable = true)
	{
		//var mPeer = Multiplayer.MultiplayerPeer;
		peer.SetTargetPeer((int)peerId);
		peer.TransferMode = reliable ? MultiplayerPeer.TransferModeEnum.Reliable : MultiplayerPeer.TransferModeEnum.Unreliable;
		peer.PutPacket(data);
	}

	public void Broadcast(byte[] data, bool reliable = true)
	{
		//var mPeer = Multiplayer.MultiplayerPeer;
		if (!Multiplayer.IsServer())
		{
			GD.PrintErr("[FATAL] Broadcast attempt on non-server peer");
			return;
		}
		peer.TransferMode = reliable ? MultiplayerPeer.TransferModeEnum.Reliable : MultiplayerPeer.TransferModeEnum.Unreliable;
		peer.SetTargetPeer((int)MultiplayerPeer.TargetPeerBroadcast);
		peer.PutPacket(data);
	}

	public void Stop()
	{
		peer?.Close();
	}

	public void StartHost(int port)
	{
		throw new NotImplementedException();
	}

	public void SendToLocalHost(byte[] data)
	{
		throw new NotImplementedException();
	}
}
