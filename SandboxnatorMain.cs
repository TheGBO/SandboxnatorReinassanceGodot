using System;
using Godot;
using NullGarel.Sandboxnator.Network;
using NullGarel.Sandboxnator.UI;
using NullGarel.Sandboxnator.WorldAndScenes;
using NullGarel.Util.GodotHelpers;
namespace NullGarel.Sandboxnator;

public partial class SandboxnatorMain : Singleton<SandboxnatorMain>
{
	[ExportCategory("Screens")]
	[Export] private CanvasLayer _mainMenu;
	[Export] private CanvasLayer _profileEditMenu;
	[Export] private CanvasLayer _settingsMenu;
	[Export] private CanvasLayer _worldMenu;
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
}
