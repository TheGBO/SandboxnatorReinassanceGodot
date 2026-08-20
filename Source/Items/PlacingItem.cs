using Godot;
using System;
using NullGarel.Sandboxnator.Audio;
using NullGarel.Sandboxnator.Entity;
namespace NullGarel.Sandboxnator.Item;

[GlobalClass]
[GodotClassName("PlacingItem")]
public partial class PlacingItem : BaseItem
{

	[ExportCategory("Preview")]
	[Export] private MeshInstance3D _previewMesh;
	[Export] private PreviewCollider _previewCollider;
	[Export] private Vector3 _previewMeshOffset = Vector3.Zero;

	private PlaceableItemData _itemData;

	public override void _Ready()
	{
		_itemData = (PlaceableItemData)itemData;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!ItemUser.ComponentParent.IsMultiplayerAuthority()) return;
		GeneratePreviewMesh();
	}

	//Client Side
	private void GeneratePreviewMesh()
	{
		PlayerInput playerInput = ItemUser.ComponentParent.componentHolder.GetComponent<PlayerInput>();

		ItemUser.isUseValid = !_previewCollider.IsColliding;

		_previewMesh.Visible = ItemUser.rayCast.IsColliding() && ItemUser.isUseValid;
		_previewMesh.GlobalPosition = GetSnappedPosition(
			ItemUser.rayCast.GetCollisionPoint(),
			ItemUser.rayCast.GetCollisionNormal(),
			playerInput.IsGridSnapMode
			) + _previewMeshOffset;
		_previewMesh.GlobalRotation = ItemUser.DesiredRotation;
		_previewCollider.GlobalPosition = _previewMesh.GlobalPosition;
		_previewCollider.GlobalRotation = _previewMesh.GlobalRotation;

	}

	//Server side
	public override void UseItem(ItemUsageArgs args)
	{
		PlayerInput playerInput = ItemUser.ComponentParent.componentHolder.GetComponent<PlayerInput>();

		if (!ItemUser.isUseValid) return;
		Node3D building = (Node3D)_itemData.BuildingScene.Instantiate();
		building.Name = Guid.NewGuid().GetHashCode().ToString();
		building.Position = GetSnappedPosition(
			args.Position,
			args.Normal,
			playerInput.IsGridSnapMode
			);

		building.Rotation = args.DesiredRotation;
		SandboxnatorMain.World.networkedEntities.CallDeferred("add_child", building);
		PlayPlacingSound(building.Position);
	}

	private void PlayPlacingSound(Vector3 placementPosition)
	{
		// When building logic places an object:
		WorldAudioManager.Instance.PlaySoundAt(((PlaceableItemData)itemData).PlacementSound, placementPosition);
	}

	private Vector3 GetSnappedPosition(Vector3 collisionPoint, Vector3 collisionNormal, bool hasGrid)
	{
		Vector3 offsetPos = collisionPoint + (collisionNormal * 0.5f);
		if (!hasGrid)
			return SandboxnatorMain.World.GetNearestSnapper(offsetPos, _itemData.SnapRange);

		//else if has a grid
		Vector3 snapped = new
		(
			Mathf.Floor(offsetPos.X + _itemData.GridSize.X),
			Mathf.Floor(offsetPos.Y + _itemData.GridSize.Y),
			Mathf.Floor(offsetPos.Z + _itemData.GridSize.Z)
		);
		return snapped;
	}

}
