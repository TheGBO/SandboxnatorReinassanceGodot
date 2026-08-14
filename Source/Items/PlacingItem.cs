using Godot;
using System;
using NullGarel.Util;
using NullGarel.Sandboxnator.WorldAndScenes;
using NullGarel.Sandboxnator.Building;
using NullGarel.Sandboxnator.Audio;
namespace NullGarel.Sandboxnator.Item;

[GlobalClass]
[GodotClassName("PlacingItem")]
public partial class PlacingItem : BaseItem
{
	[Export] public PackedScene buildingScene;
	[Export] private MeshInstance3D previewMesh;
	[Export] private PreviewCollider previewCollider;
	[Export] private float snapRange = 0.5f;
	[Export] private float normalOffset = 1;
	[Export] private Vector3 gridSize = new(0.5f, 0.5f, 0.5f);
	/// <summary>
	/// _isGrid defines if the building mode will be grid-based or snap-based
	/// true=grid
	/// false=snapper
	/// 
	/// if I ever need more flexibility, I'll make an enum.
	/// </summary>

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;

	}

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
		previewMesh.GlobalPosition = GetSnappedPosition(
			ItemUser.rayCast.GetCollisionPoint(),
			ItemUser.rayCast.GetCollisionNormal(),
			ItemUser.ComponentParent.playerInput.IsGridSnapMode
			);
		previewMesh.GlobalRotation = ItemUser.DesiredRotation;
		previewCollider.GlobalPosition = previewMesh.GlobalPosition;
		previewCollider.GlobalRotation = previewMesh.GlobalRotation;

	}

	//Server side
	public override void UseItem(ItemUsageArgs args)
	{
		if (!ItemUser.isUseValid) return;
		Node3D building = (Node3D)buildingScene.Instantiate();
		building.Name = Guid.NewGuid().GetHashCode().ToString();
		building.Position = GetSnappedPosition(
			args.Position,
			args.Normal,
			ItemUser.ComponentParent.playerInput.IsGridSnapMode
			);

		building.Rotation = args.DesiredRotation;
		SandboxnatorMain.World.networkedEntities.CallDeferred("add_child", building);
		PlayPlacingSound(building.Position);
	}

	private void PlayPlacingSound(Vector3 placementPosition)
	{
		// When building logic places an object:
		WorldAudioManager.Instance.PlaySoundAt(((PlaceableItemData)itemData).placementSound, placementPosition);
	}

	private Vector3 GetSnappedPosition(Vector3 collisionPoint, Vector3 collisionNormal, bool hasGrid)
	{
		Vector3 offsetPos = collisionPoint + (collisionNormal * 0.5f);
		if (!hasGrid)
			return SandboxnatorMain.World.GetNearestSnapper(offsetPos, snapRange);

		//else if has a grid
		Vector3 snapped = new
		(
			Mathf.Floor(offsetPos.X + gridSize.X),
			Mathf.Floor(offsetPos.Y + gridSize.Y),
			Mathf.Floor(offsetPos.Z + gridSize.Z)
		);
		return snapped;
	}

}
