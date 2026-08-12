// PlayerItemUse.cs
using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Item;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.WorldAndScenes;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.GodotHelpers;
using NullGarel.Util.IO;
using NullGarel.Util.Log;
using System;

namespace NullGarel.Sandboxnator.Entity;

public partial class PlayerItemUse : AbstractComponent<Player>
{
	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	[Export] public bool isUseValid = false;
	[Export] public PlayerItemVisuals itemVisual;

	// To be synced via MultiplayerSynchronizer
	[Export]
	public Vector3 DesiredRotation
	{
		get => _desiredRotation;
		set
		{
			NcLogger.Log("rotation set");
			_desiredRotation = value;
		}
	}
	private Vector3 _desiredRotation = new();

	private bool _canUseItem = true;
	private float _rotationIncrement = 45f;
	private BaseItem _item;

	public BaseItem Item
	{
		get => _item;
		set
		{
			_item = value;
		}
	}


	public override void _Ready()
	{
		// this component should be authority of the server.
		SetMultiplayerAuthority(1);
		SetupInput();
	}


	private void SetupInput()
	{
		ComponentParent.playerInput.RotateCCW += () => _desiredRotation.Y -= Mathf.DegToRad(_rotationIncrement);
		ComponentParent.playerInput.RotateCW += () => _desiredRotation.Y += Mathf.DegToRad(_rotationIncrement);

		ComponentParent.playerInput.UsePrimary += () => ClientUse(true);
		ComponentParent.playerInput.UseSecondary += () => ClientUse(false);

		ComponentParent.playerInput.UseIncrement += () => ComponentParent.playerItemSync.RequestCycleItem(1);
		ComponentParent.playerInput.UseDecrement += () => ComponentParent.playerItemSync.RequestCycleItem(-1);
	}

	public void ClientUse(bool primaryUsage)
	{
		if (!rayCast.IsColliding() || _item == null) return;



		RpcId(1, nameof(ServerBoundUse), primaryUsage);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ServerBoundUse(bool primaryUsage)
	{
		if (!_canUseItem || _item == null)
			return;

		int senderId = Multiplayer.GetRemoteSenderId();
		int playerId = ComponentParent.componentHolder.entityId;

		if (senderId != playerId)
		{
			NcLogger.Log($"INVALID OPERATION :: {senderId} tried to hijack item usage of {playerId}", NcLogger.LogType.Error);
			return;
		}

		//note to myself: synchronize raycast position and rotation via network, and also desiredrotation
		ItemUsageArgs args = new()
		{
			PlayerId = playerId,
			IsPrimaryUse = primaryUsage,
			DesiredRotation = _desiredRotation,
			Position = rayCast.GetCollisionPoint(),
			Normal = rayCast.GetCollisionNormal()
		};

		_item.UseItem(args);
		Rpc(nameof(ClientBoundConfirmItemUsage));

		_canUseItem = false;

		SceneTreeTimer coolDownTimer =
			GetTree().CreateTimer(_item.usageCooldown);

		coolDownTimer.Timeout += () => _canUseItem = true;
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void ClientBoundConfirmItemUsage()
	{
		itemVisual.PlayUseAnimation();
	}

	/// <summary>
	/// Receives the active item instance from PlayerItemVisuals.
	/// </summary>
	public void SetActiveItem(BaseItem item)
	{
		_item = item;
		if (_item != null)
		{
			rayCast.TargetPosition = Vector3.Forward * _item.RaycastRangeOverride;
		}
	}

}