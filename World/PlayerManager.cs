using Godot;
using System;
using NullCyan.Util;
using NullCyan.Util.GodotHelpers;
using NullCyan.Sandboxnator.Chat;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Util.Log;
namespace NullCyan.Sandboxnator.WorldAndScenes;

public partial class PlayerManager : Singleton<PlayerManager>
{
	[Export] private PackedScene playerScene;
	[Export] private Vector2 rangeOfRandomPos;

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
			// Safe to call
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
				RpcId(id, nameof(S2C_SetInitialPosition), desiredPosition, player.Name);
			}

			ChatManager.Instance.BroadcastPlayerlessMessage($"[color=(1,1,0)]{id}[/color] joined the game :3");
		}


	}

	public void RemovePlayer(long id)
	{
		PlayerProfileData pData = World.Instance.GetPlayerProfileDataByID(id);
		ChatManager.Instance.BroadcastPlayerlessMessage($"[color={pData.PlayerColor.ToHtml()}]{pData.PlayerName}[/color] left the game :C");
		World.Instance.networkedEntities.GetNode(id.ToString()).QueueFree();
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void S2C_SetInitialPosition(Vector3 position, string playerId)
	{
		if (!Multiplayer.IsServer())
		{
			Node3D playerInstance = World.Instance.networkedEntities.GetNodeOrNull<Node3D>(playerId);
			if (playerInstance == null)
			{
				NcLogger.Log("Player instance is lagging behind, delaying position change", NcLogger.LogType.Warn);
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
		NcLogger.Log($"Server placed the remote player of ID:{playerId} placed on XYZ {position} via RPC");
	}
}
