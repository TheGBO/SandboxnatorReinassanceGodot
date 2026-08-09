// PlayerItemUse.cs
using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Item;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.GodotHelpers;
using NullGarel.Util.IO;
using System;

namespace NullGarel.Sandboxnator.Entity;

public partial class PlayerItemUse : AbstractComponent<Player>
{
	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	[Export] public bool isUseValid = false;
	[Export] public PlayerItemVisuals itemVisual;
	public Vector3 desiredRotation = new();

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
		ComponentParent.playerInput.RotateCCW += () => desiredRotation.Y -= Mathf.DegToRad(_rotationIncrement);
		ComponentParent.playerInput.RotateCW += () => desiredRotation.Y += Mathf.DegToRad(_rotationIncrement);

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

		RpcId(1, nameof(ServerBoundUse), DictPack.Serialize(args));

		itemVisual.PlayUseAnimation();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ServerBoundUse(Dictionary usageArgsDict)
	{
		if (_canUseItem && _item != null)
		{
			_item.UseItem(DictPack.Deserialize<ItemUsageArgs>(usageArgsDict));
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

}