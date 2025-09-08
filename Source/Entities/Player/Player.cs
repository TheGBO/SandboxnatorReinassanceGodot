using Godot;
using System;
using Godot.Collections;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util;
using NullCyan.Sandboxnator.UI;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;

//The most basic and central class to a player.
public partial class Player : CharacterBody3D
{
	//Components
	[Export] public ComponentHolder componentHolder;
	[Export] public CharacterBody3D characterBody;
	[Export] public PlayerMovement playerMovement;
	[Export] public CameraMovement cameraMovement;
	[Export] public PlayerInput playerInput;
	[Export] public PlayerChatHud chatHud;
	[Export] public PlayerVisualSync visuals;

	//Cosmetics; TODO: Make cosmetics its own component as well.
	[Export] public PlayerProfileData profileData;


	//Individual client graphical user interface and camera holders
	[Export] public Camera3D camera;
	[Export] public Node hud;

	public override void _EnterTree()
	{
		componentHolder.entityId = int.Parse(Name);
		SetMultiplayerAuthority(componentHolder.entityId);
		
		//hide the player head model
		if (IsMultiplayerAuthority())
		{
			
			//nameTag.Visible = false;
			//install ui sound on player Hud
			UiSoundManager.Instance.TryInstallSounds(hud);
		}
		else
		{
			//disable camera and HUD from other players
			camera.Current = false;
			//make sure HUD is client side only
			hud.QueueFree();
		}

	}

}
