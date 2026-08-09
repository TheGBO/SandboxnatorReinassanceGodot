using Godot;
using NullGarel.Sandboxnator.Item;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.Log;

namespace NullGarel.Sandboxnator.Entity;

/// <summary>
/// Responsible for handling the item visuals and instantiating the proper item scene.
/// </summary>
public partial class PlayerItemVisuals : AbstractComponent<Player>
{
	[Export] public Node3D hand;
	[Export] private AnimationPlayer handAnimator;

	private BaseItem _activeItemNode;

	public override void _Ready()
	{
		base._Ready();
		ComponentParent.playerItemSync.OnItemEquipped += UpdateItemModel;
		if (!string.IsNullOrEmpty(ComponentParent.playerItemSync.CurrentItemId))
		{
			UpdateItemModel(ComponentParent.playerItemSync.CurrentItemId);
		}
	}

	private void UpdateItemModel(string itemId)
	{
		NcLogger.Log($"{nameof(PlayerItemVisuals)} :: SHOULD update as {itemId}");
		foreach (var model in hand.GetChildren())
		{
			hand.RemoveChild(model);
			model.QueueFree();
		}

		if (string.IsNullOrEmpty(itemId)) return;

		ItemData itemResource = GameRegistries.Instance.ItemRegistry.Get(itemId);
		_activeItemNode = itemResource.itemScene.Instantiate<BaseItem>();
		_activeItemNode.itemData = itemResource;
		_activeItemNode.Name = "EquippedItem";
		_activeItemNode.ItemUser = ComponentParent.playerItemUse;
		ComponentParent.playerItemUse.SetActiveItem(_activeItemNode);
		hand.AddChild(_activeItemNode);
	}

	public void PlayUseAnimation()
	{
		if (_activeItemNode != null && _activeItemNode.animateHand)
		{
			handAnimator.Stop();
			handAnimator.Play("HandUse");
		}
	}
}
