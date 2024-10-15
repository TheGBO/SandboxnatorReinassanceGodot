using Godot;
using Godot.Collections;
using System;


//TODO: fix the sync n shite
public partial class PlayerToolUse : AbstractPlayerComponent
{
	//TODO: Make multiple tool selection and sync changes

	[Export] public RayCast3D rayCast;
	[Export] public PackedScene blockScene;
	[Export] public Node3D hand;
	//The resource for loading the tool
	[Export(PropertyHint.File, "*.tres")] private string toolResourcePath;
	//runtime tool reference
	private BaseTool tool;

	public override void _Ready()
	{
		if (!parent.IsMultiplayerAuthority()) return;
		//send message to server requesting tool synchronization
		RefreshTool();
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
			RefreshTool();
		}
	}

	//Use tools
	public void Use()
	{
		if (!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		Vector3 normal = rayCast.GetCollisionNormal();
		Dictionary toolUsageArgs = new ToolUsageArgs(collisionPoint, normal, 1).ToDictionary();
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
	}

	//Dictionary conversion is needed for it is a networked function
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ServerUse(Dictionary args)
	{
		GD.Print(tool);
		tool.UseTool(ToolUsageArgs.FromDictionary(args));
	}


	private void UpdateToolModel()
	{
		Node model = hand.GetChildOrNull<Node>(0);
		if (model != null)
		{
			model.QueueFree();
		}
		ToolResource toolResource = ResourceLoader.Load<ToolResource>(toolResourcePath);
		Node loadedTool = toolResource.tool.Instantiate();
		hand.AddChild(loadedTool);
	}

	//Call to the server to synchronize the tool of a player
	//should only run on server
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void C2SToolSync(int id, string _toolResourcePath)
	{
		GD.Print($"broadcasting tool change requested from player ({id}) of tool {_toolResourcePath}");
		Rpc(nameof(S2CToolSync), id, _toolResourcePath);
	}

	//Broadcasted to all players, intended to run on client
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void S2CToolSync(int id, string _toolResourcePath)
	{
		toolResourcePath = _toolResourcePath;
		UpdateToolModel();
		GD.Print("tool sync incoming");
	}

	/// <summary>
	/// method called to request and handle tool synchronization
	/// </summary>
	public void RefreshTool()
	{
		if (!Multiplayer.IsServer())
		{
			RpcId(1, nameof(C2SToolSync), GetPlayerId(), toolResourcePath);
		}
		else
		{
			C2SToolSync(GetPlayerId(), toolResourcePath);
		}
	}

}
