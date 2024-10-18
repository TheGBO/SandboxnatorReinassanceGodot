using Godot;
using System;
using System.Linq;


//todo: make proper network manager
public partial class NetworkManager : Node3D
{
	public static NetworkManager Instance { get; private set; }
	public ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
	[Export] PackedScene playerScene;

	public override void _Ready()
	{
		Instance = this;
		Multiplayer.PeerDisconnected += LogOutPlayer;
		GD.Print("Sandboxnator protocol initialized");
		string[] args = OS.GetCmdlineArgs();
		bool dedicatedServer = args.Contains("server") && !args.Contains("client");
		GD.Print($"Dedicated server check-up: {dedicatedServer}");
		if (dedicatedServer)
		{
			//Todo: make port configurable by arguments
			HostGame(1077, true);
		}
	}

	public void HostGame(int port = 1077, bool dedicatedServer = false)
	{
		peer.CreateServer(port);
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += AddPlayer;
		AddPlayer(1);
	}

	public void JoinGame(int port = 1077, string ip = "localhost")
	{
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;

	}

	private void AddPlayer(long id = 1)
	{

		Node3D player = (Node3D)playerScene.Instantiate();
		player.SetMultiplayerAuthority((int)id);
		player.Name = id.ToString();
		//set player position
		World.Instance.neworkedEntities.CallDeferred("add_child", player);

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
				RpcId(id, nameof(SetPlayerInitialPosition), desiredPosition, player.Name);
			}
		}


	}

	private void LogOutPlayer(long id)
	{
		World.Instance.neworkedEntities.GetNode(id.ToString()).QueueFree();
	}


	[Rpc]
	private void SetPlayerInitialPosition(Vector3 position, string playerId)
	{
		if (!Multiplayer.IsServer())
		{
			Node3D playerInstance = World.Instance.neworkedEntities.GetNodeOrNull<Node3D>(playerId);
			if (playerInstance == null)
			{
				GD.Print("Player instance is lagging behind, delaying position change");
				CallDeferred(nameof(SetPlayerInitialPosition), position, playerId);
				return;
			}
			playerInstance.Position = position;
			RpcId(1, nameof(PlayerPositionServerCheck), position, playerId);
		}
	}

	//run on server to check if player position is synchronized
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void PlayerPositionServerCheck(Vector3 position, string playerId)
	{
		GD.Print($"Server placed the remote player of ID:{playerId} placed on XYZ {position} via RPC");
	}

	//todo: Server authoritative building system

}
