using Godot;
using System;

public partial class PlayerToolUse : AbstractPlayerComponent
{
	//TODO: Make multiple tool selection and sync changes
	[Export] BaseTool tool;
	[Export] RayCast3D rayCast;

    public override void _Process(double delta)
    {
		if(!parent.IsMultiplayerAuthority()) return;
        if(Input.IsActionJustPressed("use_primary"))
		{
			Use();
		}
    }

    public void Use()
	{
		//Call this on the server side
		//
		if(!rayCast.IsColliding()) return;

		Vector3 collisionPoint = rayCast.GetCollisionPoint();
		ServerUse(collisionPoint);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void ServerUse(Vector3 targetPosition)
	{
		NetworkManager.Instance.AddNut(targetPosition);
	}
}
