using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;


//TODO: fix the sync n shite

/// <summary>
/// Class that represents the player's tools and inventory, functionality and data.
/// </summary>
public partial class PlayerItemUse : AbstractPlayerComponent
{
	//TODO: Make multiple tool selection and sync changes

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
	private float rotationIncrement = 45f;

	public override void _Ready()
	{
		currentItemID = inventory[0];
		if (!parent.IsMultiplayerAuthority()) return;
		//send message to server requesting tool synchronization
		UpdateItemModelAndData();
		parent.playerInput.RotateCCW += () =>
		{
			desiredRotationY -= rotationIncrement * (Mathf.Pi / 180);
		};
		parent.playerInput.RotateCW += () =>
		{
			desiredRotationY += rotationIncrement * (Mathf.Pi / 180);
		};
		parent.playerInput.UsePrimary += Use;
		parent.playerInput.UseIncrement += () =>
		{
			CycleItem(1);
		};
		parent.playerInput.UseDecrement += () =>
		{
			CycleItem(-1);
		};
	}

	public override void _Process(double delta)
	{
		if (!parent.IsMultiplayerAuthority()) return;
		//TODO: Incorporate these inputs on PlayerInput
	}

	private void CycleItem(int increment)
	{

		inventoryIndex += increment;
		currentItemID = inventory[Mathf.Abs(inventoryIndex % inventory.Count)];

		UpdateItemModelAndData();
	}

	//Use tools
	public void Use()
	{
		if (!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		Vector3 normal = rayCast.GetCollisionNormal();
		Dictionary itemUsageArgs = new ItemUsageArgs(collisionPoint, normal, parent.playerId).ToDictionary();
		//Perform c2s RPC call if the player is a client
		//Call this on the server side if the player using the tool is the one hosting
		if (!Multiplayer.IsServer())
		{
			RpcId(1, nameof(ServerUse), itemUsageArgs);
		}
		else
		{
			ServerUse(itemUsageArgs);
		}
		if (item.animateHand)
		{
			handAnimator.Play("hand_use");
		}
	}

	//Dictionary conversion is needed for it is a networked function
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ServerUse(Dictionary args)
	{
		GD.Print(item);
		item.UseItem(ItemUsageArgs.FromDictionary(args));
	}


	private void UpdateItemModelAndData()
	{
		Node model = hand.GetChildOrNull<Node>(0);
		if (model != null)
		{
			model.QueueFree();
		}
		ItemData itemResource = ItemManager.Instance.Items[currentItemID];
		BaseItem loadedItem = itemResource.itemScene.Instantiate<BaseItem>();
		item = loadedItem;
		item.Ptu = this;
		hand.AddChild(loadedItem);
	}

	private void _on_multiplayer_synchronizer_synchronized()
	{
		//TODO: Optimize synchronization
		UpdateItemModelAndData();
	}

}
