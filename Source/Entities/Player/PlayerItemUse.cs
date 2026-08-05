// PlayerItemUse.cs
using Godot;
using NullCyan.Sandboxnator.Item;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.ComponentSystem;
using NullCyan.Util.IO;
using System;

namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerItemUse : AbstractComponent<Player>
{
	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	[Export] private AnimationPlayer handAnimator;
	[Export] public bool isUseValid = false;

	public Vector3 desiredRotation = new();

	private bool _canUseItem = true;
	private float _rotationIncrement = 45f;
	private BaseItem _item;

	public override void _Ready()
	{
		// this component should be authority of the server.
		SetMultiplayerAuthority(1);
		SetupInput();
	}

	private void SetupInput()
	{
		ComponentParent.playerInput.RotateCCW += () => desiredRotation.Y -= _rotationIncrement * (Mathf.Pi / 180);
		ComponentParent.playerInput.RotateCW += () => desiredRotation.Y += _rotationIncrement * (Mathf.Pi / 180);

		ComponentParent.playerInput.UsePrimary += () => ClientUse(true);
		ComponentParent.playerInput.UseSecondary += () => ClientUse(false);

		ComponentParent.playerInput.UseIncrement += () => ComponentParent.playerItemSync.RequestCycleItem(1);
		ComponentParent.playerInput.UseDecrement += () => ComponentParent.playerItemSync.RequestCycleItem(-1);
	}

	public void ClientUse(bool primaryUsage)
	{
		if (!rayCast.IsColliding() || _item == null) return;

		ItemUsageArgs args = new()
		{
			PlayerId = ComponentParent.componentHolder.entityId,
			DesiredRotation = desiredRotation,
			Normal = rayCast.GetCollisionNormal(),
			Position = rayCast.GetCollisionPoint(),
			IsPrimaryUse = primaryUsage
		};

		RpcId(1, nameof(ServerBoundUse), MPacker.Pack(args));

		if (_item.animateHand)
		{
			handAnimator.Stop();
			handAnimator.Play("HandUse");
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ServerBoundUse(byte[] usageArgsBytes)
	{
		if (_canUseItem && _item != null)
		{
			_item.UseItem(MPacker.Unpack<ItemUsageArgs>(usageArgsBytes));
			_canUseItem = false;

			SceneTreeTimer coolDownTimer = GetTree().CreateTimer(_item.usageCooldown);
			coolDownTimer.Timeout += () => _canUseItem = true;
		}
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

	/// <summary>
	/// broadcast state changes to clients.
	/// </summary>
	public void BroadcastItemState(byte[] stateData)
	{
		if (Multiplayer.IsServer())
		{
			Rpc(nameof(ClientBoundSyncItemState), stateData);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void ClientBoundSyncItemState(byte[] stateData)
	{
		_item?.ReceiveItemState(stateData);
	}
}