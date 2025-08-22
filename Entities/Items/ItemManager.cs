using GBOUtils;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A class that holds the data for the tools, useful for adding and synchronizing tools.
/// As a manager, this is supposed to be a singleton, however, instead of being a global autoload
/// it is only relevant in the World scene.
/// 
/// TODO: Maybe consider initializing this class when the game opens instead of the world/lobby.
/// </summary>
public partial class ItemManager : Node
{
	public static ItemManager Instance { get; private set; }
	/// <summary>
	/// A dictionary responsible for storing every single item that can be available and registered in the whole game. Identified
	/// by itemID.
	/// </summary>
	public Dictionary<string, ItemData> Items { get; private set; } = new Dictionary<string, ItemData>();
	[Export(PropertyHint.Dir)] string itemContentsPath;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		GD.Print("[color=yellow]Item content path:[/color]" + itemContentsPath);
		InitializeItems(itemContentsPath);
	}

	/// <summary>
	/// This function is responsible for loading the item data and registering them in-game.
	/// </summary>
	private void InitializeItems(string path)
	{
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
						Items.Add(res.itemID, res);
					}
				}
			}
		}
		else
		{
			GD.PrintErr($"The path: \"{path}\" returned null on file access attempt!");
		}
	}

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
