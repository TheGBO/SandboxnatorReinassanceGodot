using Godot;
using System;
using Godot.Collections;
using GBOUtils;

//The most basic and central class to a player.
public partial class Player : CharacterBody3D
{
	//Components
	//TODO: also abstract playerId as some part of networked entity thingy (e.g. NetworkedId) I suppose
	public int playerId;
	//TODO: Abstract this to be a property of a base entity
	[Export] public Node componentList;
	[Export] public PlayerMovement playerMovement;
	[Export] public CameraMovement cameraMovement;
	[Export] public PlayerInput playerInput;
	[Export] public PlayerChatHud chatHud;

	//Cosmetics; TODO: Make cosmetics its own component as well.
	[Export] public MeshInstance3D model;
	[Export] public PlayerProfileData profileData;
	[Export] public Label3D nameTag;

	//Individual client graphical user interface and camera holders
	[Export] public Camera3D camera;
	[Export] public Node hud;

	public override void _EnterTree()
	{
		playerId = int.Parse(Name);
		SetMultiplayerAuthority(playerId);
		//set components
		//TODO: Abstract this to a future base entity
		foreach (PlayerComponent component in componentList.GetChildren())
		{
			component.SetParent(this);
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
			//disable camera and HUD from other players
			camera.Current = false;
			//make sure HUD is client side only
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
