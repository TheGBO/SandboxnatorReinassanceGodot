using Godot;
using System;

public partial class PlayerToolUse : AbstractPlayerComponent
{
	//TODO: Make multiple tool selection and sync changes

	[Export] public RayCast3D rayCast;
	[Export] public PackedScene blockScene;
	[Export] public Node3D hand;
	//The resource for loading the tool
	[Export(PropertyHint.File, "*.tres")] private string _toolResourcePath;
	//runtime tool reference
	private BaseTool tool;

    public override void _Ready()
    {
        if (!parent.IsMultiplayerAuthority()) return;
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

		//Perform c2s RPC call if the player is a client
		//Call this on the server side if the player using the tool is the one hosting
		if (!Multiplayer.IsServer())
		{
			RpcId(1, nameof(ServerUse), collisionPoint, normal);
		}
		else
		{
			ServerUse(collisionPoint, normal);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void ServerUse(Vector3 position, Vector3 normal)
	{
		GD.Print(tool);
		tool.UseTool(position, normal);
	}

	public void RefreshTool()
	{
		Node model = hand.GetChildOrNull<Node>(0);
		if (model != null)
		{
			model.QueueFree();
		}
		ToolResource toolResource = ResourceLoader.Load<ToolResource>(_toolResourcePath);
		Node loadedTool = toolResource.tool.Instantiate();
		hand.AddChild(loadedTool);
		tool = (BaseTool)loadedTool;
		GD.Print(tool);
	}
}
