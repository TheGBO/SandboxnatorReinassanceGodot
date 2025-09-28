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
	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	[Export] private AnimationPlayer handAnimator;

	[Export] private Godot.Collections.Array<string> inventory = [];
	private string currentItemID;
	private int inventoryIndex;

	private BaseItem item;
	public Vector3 desiredRotation = new();
	[Export] public bool isUseValid = false;
	private float rotationIncrement = 45f;
	private bool canUseItem = true;

	public string CurrentItemID => currentItemID;

	public override void _Ready()
	{
		currentItemID = inventory.Count > 0 ? inventory[0] : "";
		SetupInput();
		UpdateItemModelAndData();

		// When a player joins, server enforces the correct item
		if (Multiplayer.IsServer() && World.HasInstance())
		{
			World.Instance.OnPlayerJoin += (long _) =>
			{
				ComponentParent.playerItemSync.ServerForceSync(currentItemID);
			};
		}
	}

	private void SetupInput()
	{
		ComponentParent.playerInput.RotateCCW += () =>
		{
			desiredRotation.Y -= rotationIncrement * (Mathf.Pi / 180);
		};
		ComponentParent.playerInput.RotateCW += () =>
		{
			desiredRotation.Y += rotationIncrement * (Mathf.Pi / 180);
		};
		ComponentParent.playerInput.UsePrimary += ClientUse;
		ComponentParent.playerInput.UseIncrement += () => RequestCycleItem(1);
		ComponentParent.playerInput.UseDecrement += () => RequestCycleItem(-1);
	}

	/// <summary>
	/// Instead of changing immediately, client asks server for item switch.
	/// </summary>
	private void RequestCycleItem(int increment)
	{
		RpcId(1, nameof(C2S_RequestCycleItem), increment);
	}

	// Set from client to run in the server.
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_RequestCycleItem(int increment)
	{
		// Server validates
		if (inventory.Count == 0) return;

		inventoryIndex += increment;
		currentItemID = inventory[Mathf.Abs(inventoryIndex % inventory.Count)];

		// Update server-side
		UpdateItemModelAndData();

		// Tell all clients
		ComponentParent.playerItemSync.ServerForceSync(currentItemID);
	}

	public void ClientUse()
	{
		if (!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		Vector3 normal = rayCast.GetCollisionNormal();

		ItemUsageArgs args = new()
		{
			PlayerId = ComponentParent.componentHolder.entityId,
			DesiredRotation = desiredRotation,
			Normal = normal,
			Position = collisionPoint
		};

		// Send usage request to server
		RpcId(1, nameof(C2S_Use), MPacker.Pack(args));

		handAnimator.Stop();
		if (item.animateHand)
			handAnimator.Play("HandUse");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_Use(byte[] usageArgsBytes)
	{
		if (canUseItem)
		{
			item.UseItem(MPacker.Unpack<ItemUsageArgs>(usageArgsBytes));
			canUseItem = false;

			SceneTreeTimer coolDownTimer = GetTree().CreateTimer(item.usageCooldown);
			coolDownTimer.Timeout += () => canUseItem = true;
		}
	}

	public void UpdateItemModelAndData()
	{
		foreach (var model in hand.GetChildren())
			model.QueueFree();

		if (string.IsNullOrEmpty(currentItemID))
			return;

		ItemData itemResource = GameRegistries.Instance.ItemRegistry.Get(currentItemID);
		item = itemResource.itemScene.Instantiate<BaseItem>();
		item.ItemUser = this;
		hand.AddChild(item);
	}

	public void SetItemFromNetwork(string itemId)
	{
		currentItemID = itemId;
		UpdateItemModelAndData();
	}
}
