using Godot;
using System;

//The most basic and central class to a player.
public partial class Player : CharacterBody3D
{
	[Export] public PlayerMovement playerMovement;
	[Export] public CameraMovement cameraMovement;
	[Export] public PlayerInput playerInput;
	[Export] public Camera3D camera;
	[Export] public Node3D model;
	[Export] public Node hud;
	[Export] public Node componentList;
	public int playerId;

	public override void _EnterTree()
	{
		playerId = int.Parse(Name);
		SetMultiplayerAuthority(playerId);
		//set components
		foreach (AbstractPlayerComponent component in componentList.GetChildren())
		{
			component.parent = this;
		}

		//hide the player head modelX
		if (IsMultiplayerAuthority())
		{
			model.Visible = false;
			//install ui sound on player Hud
			UiSoundManager.Instance.TryInstallSounds(hud);
		}
		else
		{
			camera.Current = false;
			hud.QueueFree();
		}
	}
}
