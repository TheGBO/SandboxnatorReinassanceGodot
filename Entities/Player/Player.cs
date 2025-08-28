using Godot;
using System;
using Godot.Collections;
using GBOUtils;

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

		//If this player is the authority, the client's current profile will be sent to the server in order to request synchronization.
		if (IsMultiplayerAuthority())
			UpdateProfile(PlayerProfileManager.Instance.CurrentProfile);

		if (Multiplayer.IsServer())
		{
			World.Instance.OnPlayerJoin += (id) =>
			{
				//When a player joins, the server will synchronize the data of the player who is hosting the match as well
				C2S_SyncProfile(profileData.ToDictionary());
			};

		}
	}

	/// <summary>
	/// Updates the player profile and visuals based on newProfile in the client side
	/// </summary>
	/// <param name="newProfile"></param>
	public void UpdateProfile(PlayerProfileData newProfile)
	{
		if (IsMultiplayerAuthority())
		{
			//if this is the authoriy, the data is set
			profileData = newProfile;
		}
		UpdateVisual();

		//requests server for updating
		RpcId(1, nameof(C2S_SyncProfile), newProfile.ToDictionary());
	}

	//Client requests server to synchronize its profile.
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_SyncProfile(Dictionary<string, Variant> profileDict)
	{
		PlayerProfileData receivedProfileData = PlayerProfileData.FromDictionary(profileDict);
		GD.PrintRich("[color=green](SYNC)[/color] Synchronization of player profile data");
		string hexColor = receivedProfileData.PlayerColor.ToHtml();
		GD.PrintRich($"{receivedProfileData.PlayerName}:[color={hexColor}]{hexColor}[/color]");
		Rpc(nameof(S2C_SyncProfile), profileDict);
		profileData = receivedProfileData;
		UpdateVisual();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void S2C_SyncProfile(Dictionary<string, Variant> profileDict)
	{
		profileData = PlayerProfileData.FromDictionary(profileDict);
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		nameTag.Text = profileData.PlayerName;
		nameTag.Modulate = profileData.PlayerColor;
		nameTag.OutlineModulate = ColorUtils.InvertColor(profileData.PlayerColor);
		var currentMaterial = model.GetActiveMaterial(0);
		if (currentMaterial is StandardMaterial3D stdMat)
		{
			stdMat = (StandardMaterial3D)stdMat.Duplicate();
			stdMat.AlbedoColor = profileData.PlayerColor;
			model.MaterialOverride = stdMat;
		}
	}
}
