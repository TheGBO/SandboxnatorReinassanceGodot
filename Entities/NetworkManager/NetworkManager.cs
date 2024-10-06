using Godot;
using System;


//todo: make proper network manager
public partial class NetworkManager : Node3D
{
	public static NetworkManager Instance {get; private set;}
	public ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
	[Export] PackedScene playerScene;
	[Export] PackedScene nutScene;
	//TODO: make buildings a separate script with own data structure
	[Export] Node3D buildings;
	

    public override void _Ready()
    {
		Instance = this;
        GD.Print("Sandboxnator protocol initialized");
    }

    public void HostGame(int port = 1077){
		peer.CreateServer(port);	
		Multiplayer.MultiplayerPeer = peer;
		Multiplayer.PeerConnected += AddPlayer;

		AddPlayer(1);
	}

	public void JoinGame(int port = 1077, string ip = "localhost"){
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;

	}

	private void AddPlayer(long id = 1){

		Node3D player = (Node3D)playerScene.Instantiate();
		player.SetMultiplayerAuthority((int)id);
		player.Name = id.ToString();
		//set player position
		CallDeferred("add_child", player);

		if(Multiplayer.IsServer())
		{
			GD.Seed((ulong)Time.GetUnixTimeFromSystem());
			Vector2 randPos = new Vector2(GD.Randi() % 40, GD.Randi() % 40);
			Vector3 desiredPosition = new Vector3(randPos.X, 100, randPos.Y);
			if(Multiplayer.GetUniqueId() == id)
			{
				player.Position = desiredPosition;
				GD.Print($"Server owned Player:{id} placed on XYZ {player.Position}");
			}
			else
			{
				RpcId(id, nameof(SetPlayerInitialPosition), desiredPosition, player.Name);
				GD.Print($"Alter Player:{id} placed on XYZ {player.Position} via Remote Call");
			}
		}

		
	}


	[Rpc]
	private void SetPlayerInitialPosition(Vector3 position, string playerName){
		if(!Multiplayer.IsServer()){
			Node3D playerInstance = GetNodeOrNull<Node3D>(playerName);
			if(playerInstance == null)
			{
				GD.Print("Player instance is lagging behind, delaying position change");
				CallDeferred(nameof(SetPlayerInitialPosition), position, playerName);
				return;
			}
			playerInstance.Position = position;
		}
	}

	public void AddNut(Vector3 position){
		Node3D nut = (Node3D)nutScene.Instantiate();
		CallDeferred("add_child", nut, true);
		nut.Position = position;
		
	}

}
