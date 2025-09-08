using NullCyan.Util;
using Godot;
using System;
using System.Collections.Generic;
using NullCyan.Sandboxnator.Registry;
namespace NullCyan.Sandboxnator.Item;

/// <summary>
/// A class that holds the data for the tools, useful for adding and synchronizing tools.
/// As a manager, this is supposed to be a singleton, however, instead of being a global autoload
/// it is only relevant in the World scene.
/// 

public partial class ItemRegistryManager : IRegistryManager
{
	/// TODO: Make these functionalities for loading and auto registering game assets generic and type agnostic
	private string itemContentsPath = "res://GameContent/Items";

	/// <summary>
	/// This function is responsible for loading the item data and registering them in-game.
	/// </summary>
	public void Register()
	{
		string path = itemContentsPath;
		GD.Print("[color=yellow]Item content path:[/color]" + itemContentsPath);
		GD.PrintRich("[color=green]Loading vanilla item data...[/color]");
		using var dir = DirAccess.Open(path);
		if (dir != null)
		{
			string[] directories = dir.GetDirectories();
			foreach (string itemDir in directories)
			{
				//underscore filters out abstract items
				if (!itemDir.StartsWith("_"))
				{
					string absoluteItemDir = path + "/" + itemDir;
					List<Resource> resources = LoadItemDataResources(absoluteItemDir);
					GD.Print($"Found item resource(s) at {absoluteItemDir}:");
					foreach (ItemData res in resources)
					{
						GD.Print($"Valid item resource is {res.itemID}, registering...");
						BaseItem itemScene = res.itemScene.Instantiate<BaseItem>();
						if (itemScene is PlacingItem)
						{
							GD.Print($"({res.itemID}) is a placeable building, adding to building registry as well.");
							GameRegistries.Instance.BuildingRegistry.Register(res.itemID, ((PlacingItem)itemScene).buildingScene);
						}

						//Register the item via resource
						GameRegistries.Instance.ItemRegistry.Register(res.itemID, res);
					}
				}
			}
		}
		else
		{
			GD.PrintErr($"The path: \"{path}\" returned null on file access attempt!");
		}
	}


	/// <summary>
	/// Loads the item data from game content folder in order to register them.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	private List<Resource> LoadItemDataResources(string path)
	{
		List<Resource> resources = new List<Resource>();

		DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess == null) { return null; }

		string[] files = dirAccess.GetFiles();
		if (files == null) { return null; }

		string remapSuffix = ".remap";
		string importSuffix = ".import";
		string uidSuffix = ".uid";

		foreach (string fileName in files)
		{
			string loadFileName = fileName;
			if (fileName.Contains(remapSuffix))
			{
				GD.Print($"REMAP FOUND {fileName}");
				loadFileName = StringUtils.TrimSuffix(fileName, remapSuffix);
			}

			if (!fileName.Contains(importSuffix) && !fileName.Contains(uidSuffix))
			{
				string resPath = path + "/" + loadFileName;
				Resource loadedRes = GD.Load<Resource>(resPath);
				if (loadedRes.GetType() == typeof(ItemData))
				{
					resources.Add(loadedRes);
				}
			}
		}

		return resources;
	}
}
