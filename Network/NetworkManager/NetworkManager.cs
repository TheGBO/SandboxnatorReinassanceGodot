using Godot;
using System;
using System.Linq;
using LiteNetLib;


public partial class NetworkManager : Singleton<NetworkManager>
{
	public ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
	[Export] PackedScene playerScene;

	public override void _Ready()
	{
		Multiplayer.PeerDisconnected += LogOutPlayer;
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
				RpcId(id, nameof(S2C_SetInitialPosition), desiredPosition, player.Name);
			}

			ChatManager.Instance.BroadcastPlayerlessMessage($"[color=(1,1,0)]{id}[/color] joined the game :3");
		}


	}

	private void LogOutPlayer(long id)
	{
		PlayerProfileData pData = World.Instance.GetPlayerProfileDataByID(id);
		ChatManager.Instance.BroadcastPlayerlessMessage($"[color={pData.PlayerColor.ToHtml()}]{pData.PlayerName}[/color] left the game :C");
		World.Instance.networkedEntities.GetNode(id.ToString()).QueueFree();
	}


	[Rpc]
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
			RpcId(1, nameof(C2S_PositionCheck), position, playerId);
		}
	}

	//run on server to check if player position is synchronized
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_PositionCheck(Vector3 position, string playerId)
	{
		GD.Print($"Server placed the remote player of ID:{playerId} placed on XYZ {position} via RPC");
	}

	//todo: Server authoritative building system

}
