using GBOUtils;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A class that holds the data for the tools, useful for adding and synchronizing tools.
/// As a manager, this is supposed to be a singleton, however, instead of being a global autoload
/// it is only relevant in the World scene.
/// </summary>
public partial class ItemManager : Node
{
	public static ItemManager Instance { get; private set; }
	public Dictionary<string, ItemData> Items { get; private set; } = new Dictionary<string, ItemData>();
	[Export(PropertyHint.Dir)] string itemContentsPath;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		GD.Print("[color=yellow]Item content path:[/color]" + itemContentsPath);
		InitializeItems();
	}

	private void InitializeItems()
	{
		GD.PrintRich("[color=green]Loading vanilla item data...[/color]");
		List<Resource> resources = LoadResources(itemContentsPath);

		foreach (var resource in resources)
		{
			if (resource is ItemData)
			{
				ItemData toolRes = (ItemData)resource;
				Items[toolRes.itemID] = toolRes;
				GD.Print($"Found tool data {resource}");
				GD.PrintRich($"[color=green]Adding to tool dictionary as[/color] [color=yellow]{toolRes.itemID}[/color]");
			}
		}
		GD.PrintRich($"[color=blue]Item dictionary size is:[/color] {Items.Count}");

		foreach (KeyValuePair<string, ItemData> datum in Items)
		{
			GD.Print(datum.Value.itemID);
		}
	}

	private List<Resource> LoadResources(string path)
	{
		List<Resource> resources = new List<Resource>();

		DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess == null) { return null; }

		string[] files = dirAccess.GetFiles();
		if (files == null) { return null; }

		string remapSuffix = ".remap";
		foreach (string fileName in files)
		{
			string loadFileName = fileName;
			if (fileName.Contains(remapSuffix))
			{
				GD.Print($"REMAP FOUND {fileName}");
				loadFileName = StringUtils.TrimSuffix(fileName, remapSuffix);
			}
			string resPath = path + "/" + loadFileName;
			GD.Print(resPath);
			Resource loadedRes = GD.Load<Resource>(resPath);
			resources.Add(loadedRes);
		}

		return resources;
	}
}
