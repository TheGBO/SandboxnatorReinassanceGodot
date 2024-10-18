using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A class that holds the data for the tools, useful for adding and synchronizing tools.
/// As a manager, this is supposed to be a singleton, however, instead of being a global autoload
/// it is only relevant in the World scene.
/// </summary>
public partial class ToolManager : Node
{
	public static ToolManager Instance { get; private set; }
	public Dictionary<string, ToolData> Tools { get; private set; } = new Dictionary<string, ToolData>();
	[Export(PropertyHint.Dir)] string toolContentsPath;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		InitializeTools();
	}

	private void InitializeTools()
	{
		GD.PrintRich("[color=green]Loading vanilla tool data...[/color]");
		List<Resource> resources = LoadResources(toolContentsPath);

		foreach (var resource in resources)
		{
			if (resource is ToolData)
			{
				ToolData toolRes = (ToolData)resource;
				Tools[toolRes.toolID] = toolRes;
				GD.Print($"Found tool data {resource}");
				GD.PrintRich($"[color=green]Adding to tool dictionary as[/color] [color=yellow]{toolRes.toolID}[/color]");
			}
		}
	}

	private List<Resource> LoadResources(string path)
	{
		List<Resource> resources = new List<Resource>();

		DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess == null) { return null; }

		string[] files = dirAccess.GetFiles();
		if (files == null) { return null; }

		foreach (string fileName in files)
		{
			string resPath = path + "/" + fileName;
			GD.Print(resPath);
			Resource loadedRes = GD.Load<Resource>(resPath);
			resources.Add(loadedRes);
		}

		return resources;
	}
}
