using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util;
using NullGarel.Util.Log;

namespace NullGarel.Sandboxnator.Item;

/// <summary>
/// Registers item data from the game's content database.
/// </summary>
public partial class ItemRegistryManager : IRegistryManager
{
	public void Register()
	{
		foreach (ItemData item in GameRegistries.Instance.ContentDatabase.Items)
		{
			NcLogger.Log(
				$"Valid item resource is {item.itemID}, registering...",
				NcLogger.LogType.Register
			);

			BaseItem itemScene = item.itemScene.Instantiate<BaseItem>();

			// Inject item data into the item scene.
			itemScene.itemData = item;

			if (itemScene is PlacingItem placingItem)
			{
				NcLogger.Log(
					$"({item.itemID}) is a placeable building, adding to building registry as well.",
					NcLogger.LogType.Register
				);

				GameRegistries.Instance.BuildingRegistry.Register(
					item.itemID,
					placingItem.buildingScene
				);
			}

			GameRegistries.Instance.ItemRegistry.Register(
				item.itemID,
				item
			);
		}
	}
}