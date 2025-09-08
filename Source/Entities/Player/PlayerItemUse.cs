using Godot;
using Godot.Collections;
using NullCyan.Sandboxnator.Item;
using NullCyan.Sandboxnator.Registry;
using System;
using System.Collections.Generic;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;


/// <summary>
/// Class that represents the player's tools and inventory, functionality and data.
/// </summary>
[GodotClassName(nameof(PlayerItemUse))]
[Icon("res://GameContent/Items/Hammer/HammerIcon.png")]
public partial class PlayerItemUse : AbstractComponent<Player>
{
	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	[Export] private AnimationPlayer handAnimator;
	//The resource for loading the tool
	//TODO: add inventory data structure and remove hard-coded tool ID
	[Export] private Array<string> inventory = new Array<string>();
	[Export] private string currentItemID;
	private int inventoryIndex;
	//runtime tool reference
	private BaseItem item;
	//desired rotation
	[Export] public float desiredRotationY = 0f;
	[Export] public bool isUseValid = false;
	private float rotationIncrement = 45f;
	private bool canUseItem = true;

	public override void _Ready()
	{
		currentItemID = inventory[0];
		if (!ComponentParent.IsMultiplayerAuthority()) return;
		//send message to server requesting tool synchronization
		SetupInput();
		UpdateItemModelAndData();
	}

	private void SetupInput()
	{
		ComponentParent.playerInput.RotateCCW += () =>
		{
			desiredRotationY -= rotationIncrement * (Mathf.Pi / 180);
		};
		ComponentParent.playerInput.RotateCW += () =>
		{
			desiredRotationY += rotationIncrement * (Mathf.Pi / 180);
		};
		ComponentParent.playerInput.UsePrimary += ClientUse;
		ComponentParent.playerInput.UseIncrement += () =>
		{
			CycleItem(1);
		};
		ComponentParent.playerInput.UseDecrement += () =>
		{
			CycleItem(-1);
		};
	}


	private void CycleItem(int increment)
	{
		inventoryIndex += increment;
		currentItemID = inventory[Mathf.Abs(inventoryIndex % inventory.Count)];

		UpdateItemModelAndData();
	}

	//Use tools
	public void ClientUse()
	{
		if (!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		Vector3 normal = rayCast.GetCollisionNormal();
		Dictionary itemUsageArgs = new ItemUsageArgs(collisionPoint, normal, ComponentParent.componentHolder.entityId).ToDictionary();
		//Perform c2s RPC call if the player is a client
		//Call this on the server side if the player using the tool is the one hosting

		RpcId(1, nameof(C2S_Use), itemUsageArgs);

		handAnimator.Stop();
		if (item.animateHand)
		{
			handAnimator.Play("HandUse");
		}
	}

	//Dictionary conversion is needed for it is a networked function
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_Use(Dictionary args)
	{
		if (canUseItem)
		{
			item.UseItem(ItemUsageArgs.FromDictionary(args));
			canUseItem = false;

			SceneTreeTimer coolDownTimer = GetTree().CreateTimer(item.usageCooldown);
			coolDownTimer.Timeout += () =>
			{
				canUseItem = true;
			};

			
		}
	}


	private void UpdateItemModelAndData()
	{
		Node model = hand.GetChildOrNull<Node>(0);
		if (model != null)
		{
			model.QueueFree();
		}
		ItemData itemResource = GameRegistries.Instance.ItemRegistry.Get(currentItemID);
		BaseItem loadedItem = itemResource.itemScene.Instantiate<BaseItem>();
		item = loadedItem;
		item.ItemUser = this;
		hand.AddChild(loadedItem);
	}

	private void _on_multiplayer_synchronizer_synchronized()
	{
		//TODO: Optimize synchronization
		UpdateItemModelAndData();
	}

}
