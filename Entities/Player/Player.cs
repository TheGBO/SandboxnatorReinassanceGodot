using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] PlayerMovement pmv;
	[Export] CameraMovement cmv;
	[Export] Camera3D camera;
	[Export] Node3D model;
	[Export] Node componentList;

    public override void _EnterTree()
    {
        SetMultiplayerAuthority(int.Parse(Name));
		foreach (AbstractPlayerComponent component in componentList.GetChildren())
		{
			component.parent = this;
		}
		if(IsMultiplayerAuthority()){
			model.Visible = false;
		}
		else{
			camera.Current = false;
		}
    }
}
