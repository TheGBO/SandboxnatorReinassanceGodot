using Godot;
using System;
using NullCyan.Util;
using NullCyan.Sandboxnator.WorldAndScenes;
namespace NullCyan.Sandboxnator.Item;

[GlobalClass]
[GodotClassName("PlacingItem")]
public partial class PlacingItem : BaseItem
{
	[Export] private PackedScene buildingScene;
	[Export] private MeshInstance3D previewMesh;
	[Export] private PreviewCollider previewCollider;
	[Export] private float snapRange;
	[Export] private float normalOffset = 1;
	//Sync C2S
	

	public override void _PhysicsProcess(double delta)
	{
		if (!ItemUser.ComponentParent.IsMultiplayerAuthority()) return;
		GeneratePreviewMesh();
	}

	//Client Side
	private void GeneratePreviewMesh()
	{
		ItemUser.isUseValid = !previewCollider.IsColliding;

		previewMesh.Visible = ItemUser.rayCast.IsColliding() && ItemUser.isUseValid;
		previewMesh.GlobalPosition = GetSnappedPosition(ItemUser.rayCast.GetCollisionPoint(), ItemUser.rayCast.GetCollisionNormal());
		previewMesh.GlobalRotation = new Vector3(0, ItemUser.desiredRotationY, 0);
		previewCollider.GlobalPosition = previewMesh.GlobalPosition;
		previewCollider.GlobalRotation = previewMesh.GlobalRotation;

	}

	//run on server
	//TODO: Reformulate method of passing tool usage data to P.T.U, probably just by getting them directly from the player
	public override void UseItem(ItemUsageArgs args)
	{
		if (!ItemUser.isUseValid) return;
		//GD.Print($"block placed by {args.PlayerId}");
		Node3D building = (Node3D)buildingScene.Instantiate();
		building.Name = Guid.NewGuid().GetHashCode().ToString();
		building.Position = GetSnappedPosition(args.Position, args.Normal);
		building.Rotation = new Vector3(0, ItemUser.desiredRotationY, 0);
		World.Instance.networkedEntities.CallDeferred("add_child", building);

	}

	private Vector3 GetSnappedPosition(Vector3 collisionPoint, Vector3 collisionNormal)
	{
		return World.Instance.GetNearestSnapper(collisionPoint + (collisionNormal / 2) * normalOffset, snapRange);
		//return World.Instance.GetNearestSnapper(collisionPoint, snapRange);
	}
}
