using Godot;
using System;
using System.Linq;
using LiteNetLib;
using System.IO;
using System.Security.Cryptography.X509Certificates;

//TODO: Make network manager class modular and game agnostic
public partial class NetworkManager : Singleton<NetworkManager>
{

	private ITransport transport;
	private PacketHandlerRegistry handlerRegistry;

	public int LocalId { get; set; } 
	public bool IsServer => transport.IsServer;
	public bool IsNetConnected => transport.IsNetConnected;
	public string GameVersion { get; private set; }



	public override void _Ready()
	{
		//Initialize back-end
		GameVersion = ProjectSettings.GetSetting("application/config/version").AsString();
		RegisterPackets();
		SetUpTransport();

		string transportNameInfo = nameof(transport);

		GD.Print($"Sandboxnator multiplayer protocol v{GameVersion} initialized using transport: {transportNameInfo}");
		string[] args = OS.GetCmdlineArgs();
		bool isDedicatedServer = args.Contains("server") && !args.Contains("client");
		GD.Print($"Dedicated server check-up: {isDedicatedServer}");

		if (isDedicatedServer)
		{
			//TODO: make port configurable by arguments
			HostGame(1077, true);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!transport.IsNetConnected)
			return;

		try
		{
			transport.PollTransportEvents();
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[LiteNetLibTransport] PollEvents failed: {ex}");
		}
    }

#region CONFIGURATION
	private void SetUpTransport()
	{
		transport = LiteNetLibTransport.Instance;
		LocalId = transport.LocalPeerId;
		//transport.OnPeerDisconnected += LogOutPlayer;
		//transport.OnPeerConnected += AddPlayer;

		transport.PeerConnectedEvent += (id) =>
		{
			if (transport.IsClient && !transport.IsServer)
			{
				GD.Print("Connected as a client to server");
			}
		};

		transport.PeerDisconnectedEvent += (id) =>
		{
			GD.Print($"{id} Disconnected");
		};

		transport.DataReceivedEvent += (peerId, data) =>
		{
			using (MemoryStream ms = new MemoryStream(data))
			using (BinaryReader r = new BinaryReader(ms))
			{
				ushort packetId = r.ReadUInt16();

				try
				{
					IPacket packet = PacketFactory.CreatePacket(packetId);
					packet.Deserialize(r);
					GD.Print($"Packet received is of type: [{packet}]");
					handlerRegistry.Handle(peerId, packet);
				}
				catch (Exception ex)
				{
					GD.PushError($"Failed to process packet: {ex.Message}");
				}
			}
			//TODO: Actual packet handler
			GD.Print($"[{peerId}] sent data: {data}");
		};
	}

	private void RegisterPackets()
	{
		handlerRegistry = new();

		//register packets
		PacketFactory.Register<WelcomePacket>(1);
		PacketFactory.Register<SpawnPlayerPacket>(2);
		//register handlers
		handlerRegistry.RegisterPacketHandler(new WelcomeHandler());
		handlerRegistry.RegisterPacketHandler(new SpawnPlayerHandler());
	}

#endregion


	public void HostGame(int port = 1077, bool dedicatedServer = false)
	{
		/*
		peer.CreateServer(port);
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += AddPlayer;
		AddPlayer(1);
		*/
		transport.StartHost(port);
		//transport.OnPeerConnected += AddPlayer;
		transport.PeerConnectedEvent += id =>
		{
			GD.Print($"Peer of ID: {id} connected remotely");
			SpawnPlayerPacket spawnPlayerPacket = new SpawnPlayerPacket{
				PlayerId = (int)id,
				PlayerPosition = new Vector3(0, 10, 0),
				PlayerRotation = new Quaternion(0, 0, 0, 0)
			};
			transport.BroadCastPacket(spawnPlayerPacket, true);
		};
		//AddPlayer(1);
	}

	public void JoinGame(int port = 1077, string ip = "127.0.0.1")
	{
		/*
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;
		*/
		transport.StartClient(ip, port);
	}

	private void AddPlayer(long id = 1)
	{
		/*
		Node3D player = (Node3D)playerScene.Instantiate();
		player.SetMultiplayerAuthority((int)id);
		player.Name = id.ToString();
		//set player position
		World.Instance.networkedEntities.CallDeferred("add_child", player);
		World.Instance.OnPlayerJoin?.Invoke(id);

		if (Multiplayer.IsServer())
		{
			GD.Seed((ulong)Time.GetUnixTimeFromSystem());
			Vector2 randPos = new Vector2(GD.Randi() % 40, GD.Randi() % 40);
			Vector3 desiredPosition = new Vector3(randPos.X, 20, randPos.Y);
			if (Multiplayer.GetUniqueId() == id)
			{
				player.Position = desiredPosition;
				GD.Print($"Server owned Player:{id} placed on XYZ {player.Position}");
			}
			else
			{
				//send a RPC to the player who connected to set their position
				//RpcId(id, nameof(S2C_SetInitialPosition), desiredPosition, player.Name);
			}

			ChatManager.Instance.BroadcastPlayerlessMessage($"[color=(1,1,0)]{id}[/color] joined the game :3");
		}
		*/


	}

	private void LogOutPlayer(long id)
	{
		PlayerProfileData pData = World.Instance.GetPlayerProfileDataByID(id);
		ChatManager.Instance.BroadcastPlayerlessMessage($"[color={pData.PlayerColor.ToHtml()}]{pData.PlayerName}[/color] left the game :C");
		World.Instance.networkedEntities.GetNode(id.ToString()).QueueFree();
	}


#region DEPR.RPC CALLS
	//[Rpc]
	private void S2C_SetInitialPosition(Vector3 position, string playerId)
	{
		if (!Multiplayer.IsServer())
		{
			Node3D playerInstance = World.Instance.networkedEntities.GetNodeOrNull<Node3D>(playerId);
			if (playerInstance == null)
			{
				GD.Print("Player instance is lagging behind, delaying position change");
				CallDeferred(nameof(S2C_SetInitialPosition), position, playerId);
				return;
			}
			playerInstance.Position = position;
			//RpcId(1, nameof(C2S_PositionCheck), position, playerId);
		}
	}

	//run on server to check if player position is synchronized
	//[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_PositionCheck(Vector3 position, string playerId)
	{
		GD.Print($"Server placed the remote player of ID:{playerId} placed on XYZ {position} via RPC");
	}
#endregion
}
