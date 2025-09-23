using NullCyan.Util;
using Godot;
using System;
using System.Collections.Generic;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.Log;
using NullCyan.Util.IO;
namespace NullCyan.Sandboxnator.Item;

/// <summary>
/// A class that holds the data for the tools, useful for adding and synchronizing tools.
/// As a manager, this is supposed to be a singleton, however, instead of being a global autoload
/// it is only relevant in the World scene.
/// 

public partial class ItemRegistryManager : IRegistryManager
{
	/// TODO:PROGRESS: Make these functionalities for loading and auto registering game assets generic and type agnostic
	private readonly string itemContentsPath = "res://GameContent/Items";

	/// <summary>
	/// This function is responsible for loading the item data and registering them in-game.
	/// </summary>
	public void Register()
	{
		List<Resource> itemResources = ResourceIO.GetResources<ItemData>(itemContentsPath);
		foreach (ItemData res in itemResources)
		{
			NcLogger.Log($"Valid item resource is {res.itemID}, registering...", NcLogger.LogType.Register);
			BaseItem itemScene = res.itemScene.Instantiate<BaseItem>();
			if (itemScene is PlacingItem)
			{
				NcLogger.Log($"({res.itemID}) is a placeable building, adding to building registry as well.", NcLogger.LogType.Register);
				GameRegistries.Instance.BuildingRegistry.Register(res.itemID, ((PlacingItem)itemScene).buildingScene);
			}

			//Register the item via resource
			GameRegistries.Instance.ItemRegistry.Register(res.itemID, res);
		}
	}




}
