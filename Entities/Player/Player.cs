using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public PlayerMovement pmv;
	[Export] public CameraMovement cmv;
	[Export] public Camera3D camera;
	[Export] public Node3D model;
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
		}
		else
		{
			camera.Current = false;
		}
	}
}
