using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;


//TODO: fix the sync n shite

/// <summary>
/// Class that represents the player's tools and inventory, functionality and data.
/// </summary>
public partial class PlayerToolUse : AbstractPlayerComponent
{
	//TODO: Make multiple tool selection and sync changes

	[Export] public RayCast3D rayCast;
	[Export] public Node3D hand;
	//The resource for loading the tool
	//TODO: add inventory data structure and remove hard-coded tool ID
	[Export] private string currentToolID = "tool_hammer";
	[Export] private Array<string> inventory;
	private int inventoryIndex;
	[Export] private AnimationPlayer handAnimator;
	//runtime tool reference
	private BaseTool tool;

	public override void _Ready()
	{
		if (!parent.IsMultiplayerAuthority()) return;
		//send message to server requesting tool synchronization
		UpdateToolModelAndData();
	}

	public override void _Process(double delta)
	{
		if (!parent.IsMultiplayerAuthority()) return;

		if (Input.IsActionJustPressed("use_primary"))
		{
			Use();
		}

		if (Input.IsActionJustPressed("dbg_tool_refresh"))
		{
			if (inventoryIndex < inventory.Count - 1)
			{
				inventoryIndex++;
			}
			else
			{
				inventoryIndex = 0;
			}
			currentToolID = inventory[inventoryIndex];

			UpdateToolModelAndData();
		}
	}

	//Use tools
	public void Use()
	{
		if (!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		Vector3 normal = rayCast.GetCollisionNormal();
		Dictionary toolUsageArgs = new ToolUsageArgs(collisionPoint, normal, parent.playerId).ToDictionary();
		//Perform c2s RPC call if the player is a client
		//Call this on the server side if the player using the tool is the one hosting
		if (!Multiplayer.IsServer())
		{
			RpcId(1, nameof(ServerUse), toolUsageArgs);
		}
		else
		{
			ServerUse(toolUsageArgs);
		}
		if (tool.animateHand)
		{
			handAnimator.Play("hand_use");
		}
	}

	//Dictionary conversion is needed for it is a networked function
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ServerUse(Dictionary args)
	{
		GD.Print(tool);
		tool.UseTool(ToolUsageArgs.FromDictionary(args));
	}


	private void UpdateToolModelAndData()
	{
		Node model = hand.GetChildOrNull<Node>(0);
		if (model != null)
		{
			model.QueueFree();
		}
		ToolData toolResource = ToolManager.Instance.Tools[currentToolID];
		BaseTool loadedTool = toolResource.toolScene.Instantiate<BaseTool>();
		tool = loadedTool;
		tool.Ptu = this;
		hand.AddChild(loadedTool);
	}

	private void _on_multiplayer_synchronizer_synchronized()
	{
		//TODO: Optimize synchronization
		UpdateToolModelAndData();
	}

}
