using Godot;
using Godot.Collections;
using NullGarel.Util;
using NullGarel.Util.GodotHelpers;
using NullGarel.Sandboxnator.Chat;
using NullGarel.Sandboxnator.Entity;
using NullGarel.Util.Log;
using NullGarel.Sandboxnator.Network;
using System;
namespace NullGarel.Sandboxnator.WorldAndScenes;

public partial class PlayerManager : Singleton<PlayerManager>
{
	[Export] private PackedScene playerScene;
	[Export] private Vector2 rangeOfRandomPos;

	public override void _Ready()
	{
		// this component should be authority of the server.
		SetMultiplayerAuthority(1);
	}

	public void SendHandShake()
	{
		if (Multiplayer.IsServer())
			return;

		Dictionary profileDict = DictPack.Serialize(PlayerProfileManager.Instance.CurrentProfile);
		RpcId(1, nameof(ServerBoundHandshake), profileDict);
	}

	public void AddPlayer(long id = 1)
	{

		Node3D player = (Node3D)playerScene.Instantiate();
		player.SetMultiplayerAuthority((int)id);
		player.Name = id.ToString();
		//set player position
		if (World.Instance == null)
		{
			NcLogger.Log("World.Instance is null!", NcLogger.LogType.Error);
		}
		else if (World.Instance.networkedEntities == null)
		{
			NcLogger.Log("World.Instance.networkedEntities is null!", NcLogger.LogType.Error);
		}
		else if (player == null)
		{
			NcLogger.Log("player is null!", NcLogger.LogType.Error);
		}
		else
		{
			World.Instance.networkedEntities.CallDeferred("add_child", player);
		}

		World.Instance.OnPlayerJoin?.Invoke(id);

		if (Multiplayer.IsServer())
		{
			GD.Seed((ulong)Time.GetUnixTimeFromSystem());
			Vector2 randPos = new(GD.Randi() % rangeOfRandomPos.X, GD.Randi() % rangeOfRandomPos.Y);
			Vector3 desiredPosition = new(randPos.X, 20, randPos.Y);
			if (Multiplayer.GetUniqueId() == id)
			{
				player.Position = desiredPosition;
				NcLogger.Log($"Server owned Player:{id} placed on XYZ {player.Position}");
			}
			else
			{
				//send a RPC to the player who connected to set their position
				RpcId(id, nameof(ClientBoundSetInitialPosition), desiredPosition, player.Name);
			}


		}


	}

	public void RemovePlayer(long id)
	{
		if (World.Instance == null)
		{
			NcLogger.Log(
				$"RemovePlayer({id}): World.Instance is null.",
				NcLogger.LogType.Warn);
			return;
		}

		PlayerProfileData pData = World.Instance.GetPlayerProfileDataByID(id);

		if (pData != null)
		{
			if (ChatManager.Instance != null)
			{
				ChatManager.Instance.BroadcastPlayerlessMessage(
					$"[color={pData.PlayerColor.ToHtml()}]{pData.PlayerName}[/color] left the game :C");
			}
		}
		else
		{
			NcLogger.Log(
				$"RemovePlayer({id}): Player profile not found. It may already be cleaned up.",
				NcLogger.LogType.Warn);
		}

		if (World.Instance.networkedEntities != null)
		{
			Node player = World.Instance.networkedEntities.GetNodeOrNull(id.ToString());

			if (player != null)
			{
				player.QueueFree();
			}
			else
			{
				NcLogger.Log(
					$"RemovePlayer({id}): Player node not found. It may already be freed.",
					NcLogger.LogType.Warn);
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ServerBoundHandshake(Dictionary profileDict)
	{
		if (!Multiplayer.IsServer()) return;

		int remoteId = Multiplayer.GetRemoteSenderId();
		PlayerProfileData profileData = DictPack.Deserialize<PlayerProfileData>(profileDict);

		AddPlayer(remoteId);

		NcLogger.Log($"SERVER::Handshake received from {remoteId} ; {profileData.PlayerName}");
		ChatManager.Instance.BroadcastPlayerlessMessage($"[color={profileData.PlayerColor.ToHtml()}]{profileData.PlayerName}[/color] joined the game :3");

		RpcId(remoteId, nameof(ClientBoundHandshake));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void ClientBoundHandshake()
	{
		if (Multiplayer.IsServer()) return;

		NcLogger.Log("Handshake acknowledged on client");
		NetworkManager.Instance.NotifyConnectionEstablished();
	}


	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void ClientBoundSetInitialPosition(Vector3 position, string playerId)
	{
		if (!Multiplayer.IsServer())
		{
			Node3D playerInstance = World.Instance.networkedEntities.GetNodeOrNull<Node3D>(playerId);
			if (playerInstance == null)
			{
				NcLogger.Log("Player instance is lagging behind, delaying position change", NcLogger.LogType.Warn);
				CallDeferred(nameof(ClientBoundSetInitialPosition), position, playerId);
				return;
			}
			playerInstance.Position = position;
			RpcId(1, nameof(ServerBoundPositionCheck), position, playerId);
		}
	}

	// Call from a client to run on server to check if player position is synchronized
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ServerBoundPositionCheck(Vector3 position, string playerId)
	{
		NcLogger.Log($"Server placed the remote player of ID:{playerId} placed on XYZ {position} via RPC");
	}

	internal void OnPeerConnected(long id)
	{
		NcLogger.Log($"Peer connected, waiting for handshake, connection id is {id}");
	}
}
