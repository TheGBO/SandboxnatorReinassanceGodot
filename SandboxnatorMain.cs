using System;
using Godot;
using NullGarel.Sandboxnator.Network;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Sandboxnator.UI;
using NullGarel.Sandboxnator.WorldAndScenes;
using NullGarel.Util.GodotHelpers;
namespace NullGarel.Sandboxnator;

public partial class SandboxnatorMain : Singleton<SandboxnatorMain>, ISettingsLoader
{
	[ExportCategory("Screens")]
	[Export] private CanvasLayer _mainMenu;
	[Export] private CanvasLayer _profileEditMenu;
	[Export] private CanvasLayer _settingsMenu;
	[Export] private CanvasLayer _worldMenu;
	[ExportCategory("Debug UI")]
	[Export] private CanvasLayer _debugLayer;
	[ExportCategory("World")]
	[Export] private Node3D _worldContainer;
	[Export] private PackedScene _worldScene;

	private World _world;

	public static World World => Instance?._world;

	private Node[] Screens => [
		_mainMenu,
		_profileEditMenu,
		_settingsMenu,
		_world,
		_worldMenu
	];

	public override void _Ready()
	{
		UpdateSettingsData();
		GameRegistries.Instance.OnSettingsChanged += UpdateSettingsData;
		//make the main menu the defualty mwhen the game boots up
		ActivateMainMenu();
		UiSoundManager.Instance.TryInstallSounds();
	}

	public void Activate(Node screen)
	{
		foreach (Node candidate in Screens)
		{
			if (candidate == null || !IsInstanceValid(candidate))
				continue;

			bool visibilityCondition = candidate == screen;

			if (candidate is CanvasLayer cl)
				cl.Visible = visibilityCondition;
			else if (candidate is CanvasItem citem)
				citem.Visible = visibilityCondition;
			else if (candidate is Node3D n3d)
				n3d.Visible = visibilityCondition;
			else
				throw new InvalidOperationException(
					$"{candidate.GetType().Name} has no supported visibility property.");
		}
	}

	public void LoadWorld()
	{
		if (_world != null && IsInstanceValid(_world))
			return;

		_world = _worldScene.Instantiate<World>();
		_world.Name = "world";
		_worldContainer.AddChild(_world);
	}

	public void UnloadWorld()
	{
		if (_world == null || !IsInstanceValid(_world))
			return;

		_world.QueueFree();
		_world = null;
		// i wont call activate world here because it's responsibilyit opf the handshake
	}

	public void LeaveWorld(bool returnToMainMenu = true)
	{
		NetworkManager.Instance.QuitConnection();

		UnloadWorld();

		if (returnToMainMenu)
			ActivateMainMenu();
		else
			ActivateWorldMenu();
	}

	public void ActivateWorld() => Activate(_world);
	public void ActivateMainMenu() => Activate(_mainMenu);
	public void ActivateProfileEditMenu() => Activate(_profileEditMenu);
	public void ActivateSettingsMenu() => Activate(_settingsMenu);
	public void ActivateWorldMenu() => Activate(_worldMenu);

	//Apply game-wide settings data.
	public void UpdateSettingsData()
	{
		GameSettingsData d = GameRegistries.Instance.SettingsData;
		GD.Print($"SETTINGs CHANGEDC {d.FullScreen}");

		//full screen
		DisplayServer.WindowMode windowMode = d.FullScreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;
		DisplayServer.WindowSetMode(windowMode);

		//debug UI
		_debugLayer.Visible = d.DebugUiEnabled;
	}
}
