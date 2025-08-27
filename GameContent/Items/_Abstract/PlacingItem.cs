using Godot;
using System;

[GlobalClass]
public partial class PlacingItem : BaseItem
{
	[Export] private PackedScene buildingScene;
	[Export] private Node3D previewMesh;
	[Export] private float snapRange;
	[Export] private float normalOffset = 1;

	public override void _Process(double delta)
	{
		if (!Ptu.parent.IsMultiplayerAuthority()) return;
		GeneratePreviewMesh();
	}

	private void GeneratePreviewMesh()
	{
		previewMesh.Visible = Ptu.rayCast.IsColliding();
		previewMesh.GlobalPosition = GetSnappedPosition(Ptu.rayCast.GetCollisionPoint(), Ptu.rayCast.GetCollisionNormal());
		previewMesh.GlobalRotation = new Vector3(0, Ptu.desiredRotationY, 0);
	}

	//run on server
	//TODO: Reformulate method of passing tool usage data to P.T.U, probably just by getting them directly from the player
	public override void UseItem(ItemUsageArgs args)
	{
		GD.Print($"block placed by {args.PlayerId}");
		Node3D building = (Node3D)buildingScene.Instantiate();
		building.Name = Guid.NewGuid().GetHashCode().ToString();
		building.Position = GetSnappedPosition(args.Position, args.Normal);
		building.Rotation = new Vector3(0, Ptu.desiredRotationY, 0);
		World.Instance.networkedEntities.CallDeferred("add_child", building);

	}

	private Vector3 GetSnappedPosition(Vector3 collisionPoint, Vector3 collisionNormal)
	{
		return World.Instance.GetNearestSnapper(collisionPoint + (collisionNormal / 2) * normalOffset, snapRange);
		//return World.Instance.GetNearestSnapper(collisionPoint, snapRange);
	}
}
