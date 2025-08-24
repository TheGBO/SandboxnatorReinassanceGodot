using Godot;
using System;
using Godot.Collections;

//The most basic and central class to a player.
public partial class Player : CharacterBody3D
{
	[Export] public PlayerMovement playerMovement;
	[Export] public CameraMovement cameraMovement;
	[Export] public PlayerInput playerInput;
	[Export] public PlayerChatHud chatHud;
	[Export] public Camera3D camera;
	[Export] public MeshInstance3D model;
	[Export] public Node hud;
	[Export] public Node componentList;
	[Export] public PlayerProfileData profileData;
	[Export] public Label3D nameTag;
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

		//hide the player head model
		if (IsMultiplayerAuthority())
		{
			model.Visible = false;
			//nameTag.Visible = false;
			//install ui sound on player Hud
			UiSoundManager.Instance.TryInstallSounds(hud);
		}
		else
		{
			camera.Current = false;
			hud.QueueFree();
		}

		if (IsMultiplayerAuthority())
			UpdateProfile(PlayerProfileManager.Instance.CurrentProfile);

		if (Multiplayer.IsServer())
		{
			World.Instance.OnPlayerJoin += (id) =>
			{
				ServerSyncProfile(profileData.ToDictionary());
			};
			
		}
	}

	public void UpdateProfile(PlayerProfileData newProfile)
	{
		if (IsMultiplayerAuthority())
		{
			profileData = newProfile;
		}
		UpdateVisual();

		if (!Multiplayer.IsServer())
			RpcId(1, nameof(ServerSyncProfile), newProfile.ToDictionary());
		else
			ServerSyncProfile(newProfile.ToDictionary());
		
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ServerSyncProfile(Dictionary<string, Variant> profileDict)
	{
		PlayerProfileData receivedProfileData = PlayerProfileData.FromDictionary(profileDict);
		GD.PrintRich("[color=green](SYNC)[/color] Synchronization of player profile data");
		string hexColor = receivedProfileData.PlayerColor.ToHtml();
		GD.PrintRich($"{receivedProfileData.PlayerName}:[color={hexColor}]{hexColor}[/color]");
		Rpc(nameof(ClientSyncProfile), profileDict);
		profileData = receivedProfileData;
		UpdateVisual();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ClientSyncProfile(Dictionary<string, Variant> profileDict)
	{
		profileData = PlayerProfileData.FromDictionary(profileDict);
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		nameTag.Text = profileData.PlayerName;
		nameTag.Modulate = profileData.PlayerColor;
		var currentMaterial = model.GetActiveMaterial(0);
		if (currentMaterial is StandardMaterial3D stdMat)
		{
			stdMat = (StandardMaterial3D)stdMat.Duplicate();
			stdMat.AlbedoColor = profileData.PlayerColor;
			model.MaterialOverride = stdMat;
		}
	}
}
