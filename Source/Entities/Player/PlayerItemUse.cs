using Godot;
using NullCyan.Sandboxnator.Item;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util.ComponentSystem;
using NullCyan.Util.IO;
using NullCyan.Util.Log;
using System;

namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerItemUse : AbstractComponent<Player>
{
	// World interaction
	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	[Export] private AnimationPlayer handAnimator;
	//validation related to client-side factors such as controls andworld position
	[Export] public bool isUseValid = false;
	public Vector3 desiredRotation = new();
	// validation to server-side factors such as cooldown
	private bool _canUseItem = true;
	private float _rotationIncrement = 45f;

	// Inventory management
	[Export] private Godot.Collections.Array<string> inventory = [];
	private string _currentItemID;
	private int _inventoryIndex;
	private BaseItem _item;
	public Action<string> OnItemChanged;


	public string CurrentItemID => _currentItemID;

	public override void _Ready()
	{
		SetupInput();
		UpdateItemModelAndData();
		OnItemChanged?.Invoke(_currentItemID);
		OnItemChanged += UpdateRaycastRange;
	}

	private void UpdateRaycastRange(string _)
	{
		rayCast.TargetPosition = Vector3.Forward * _item.RaycastRangeOverride;
		NcLogger.Log($"updating raycast range to {rayCast.TargetPosition}");
	}

	private void SetupInput()
	{
		ComponentParent.playerInput.RotateCCW += () =>
		{
			desiredRotation.Y -= _rotationIncrement * (Mathf.Pi / 180);
		};
		ComponentParent.playerInput.RotateCW += () =>
		{
			desiredRotation.Y += _rotationIncrement * (Mathf.Pi / 180);
		};
		ComponentParent.playerInput.UsePrimary += ClientUsePrimary;
		ComponentParent.playerInput.UseSecondary += ClientUseSecondary;
		ComponentParent.playerInput.UseIncrement += () => RequestCycleItem(1);
		ComponentParent.playerInput.UseDecrement += () => RequestCycleItem(-1);
	}

	private void RequestCycleItem(int increment)
	{
		RpcId(1, nameof(C2S_RequestCycleItem), increment);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_RequestCycleItem(int increment)
	{
		if (!Multiplayer.IsServer()) return;
		if (inventory.Count == 0) return;

		_inventoryIndex += increment;
		string nextItemId = inventory[Mathf.Abs(_inventoryIndex % inventory.Count)];
		SetItemFromNetwork(nextItemId);

		Rpc(nameof(S2C_ConfirmItemChange), nextItemId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void S2C_ConfirmItemChange(string itemId)
	{
		SetItemFromNetwork(itemId);
	}

	public void ClientUsePrimary() => ClientUse(true);
	public void ClientUseSecondary() => ClientUse(false);

	public void ClientUse(bool primaryUsage)
	{
		if (!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		Vector3 normal = rayCast.GetCollisionNormal();

		ItemUsageArgs args = new()
		{
			PlayerId = ComponentParent.componentHolder.entityId,
			DesiredRotation = desiredRotation,
			Normal = normal,
			Position = collisionPoint,
			IsPrimaryUse = primaryUsage
		};

		// Send usage request to server
		RpcId(1, nameof(C2S_Use), MPacker.Pack(args));

		handAnimator.Stop();
		if (_item.animateHand)
			handAnimator.Play("HandUse");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_Use(byte[] usageArgsBytes)
	{
		if (_canUseItem)
		{
			_item.UseItem(MPacker.Unpack<ItemUsageArgs>(usageArgsBytes));
			_canUseItem = false;

			SceneTreeTimer coolDownTimer = GetTree().CreateTimer(_item.usageCooldown);
			coolDownTimer.Timeout += () => _canUseItem = true;
		}
	}

	public void UpdateItemModelAndData()
	{
		foreach (var model in hand.GetChildren())
		{
			hand.RemoveChild(model);
			model.QueueFree();
		}

		if (string.IsNullOrEmpty(_currentItemID))
			return;

		ItemData itemResource = GameRegistries.Instance.ItemRegistry.Get(_currentItemID);
		_item = itemResource.itemScene.Instantiate<BaseItem>();
		_item.Name = "EquippedItem";
		_item.ItemUser = this;
		hand.AddChild(_item);
	}

	public void SetItemFromNetwork(string itemId)
	{
		_currentItemID = itemId;
		OnItemChanged?.Invoke(_currentItemID);
		UpdateItemModelAndData();
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false)]
	private void S2C_SyncInitialItem(string itemId)
	{
		SetItemFromNetwork(itemId);
	}
}
