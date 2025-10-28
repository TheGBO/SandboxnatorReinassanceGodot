using Godot;
using System;
using NullCyan.Util;
using NullCyan.Sandboxnator.WorldAndScenes;
namespace NullCyan.Sandboxnator.Item;

[GlobalClass]
[GodotClassName("PlacingItem")]
public partial class PlacingItem : BaseItem
{
	[Export] public PackedScene buildingScene;
	[Export] private MeshInstance3D previewMesh;
	[Export] private PreviewCollider previewCollider;
	[Export] private float snapRange;
	[Export] private float normalOffset = 1;
	private bool _isGrid = false;


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
		previewMesh.GlobalPosition = GetSnappedPosition(ItemUser.rayCast.GetCollisionPoint(), ItemUser.rayCast.GetCollisionNormal(), _isGrid);
		previewMesh.GlobalRotation = ItemUser.desiredRotation;
		previewCollider.GlobalPosition = previewMesh.GlobalPosition;
		previewCollider.GlobalRotation = previewMesh.GlobalRotation;

	}

	//Server side
	public override void UseItem(ItemUsageArgs args)
	{
		if (!ItemUser.isUseValid) return;
		Node3D building = (Node3D)buildingScene.Instantiate();
		building.Name = Guid.NewGuid().GetHashCode().ToString();
		building.Position = GetSnappedPosition(args.Position, args.Normal, _isGrid);
		// CONSIDER: desiredRotation could be an element of ItemUsageArgs too.
		building.Rotation = args.DesiredRotation;
		World.Instance.networkedEntities.CallDeferred("add_child", building);

	}

	private Vector3 GetSnappedPosition(Vector3 collisionPoint, Vector3 collisionNormal, bool hasGrid)
	{
		Vector3 offsetPos = collisionPoint + (collisionNormal * 0.5f);
		if (!hasGrid)
			return World.Instance.GetNearestSnapper(offsetPos, snapRange);

		//else if has a grid
		Vector3 snapped = new
		(
			Mathf.Floor(offsetPos.X + 0.5f),
			Mathf.Floor(offsetPos.Y + 0.5f),
			Mathf.Floor(offsetPos.Z + 0.5f)
		);
		return snapped;
	}

}
