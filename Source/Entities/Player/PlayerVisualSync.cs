using Godot.Collections;
using Godot;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;

/// <summary>
/// Handles the visual effects and their multiplayer synchronization.
/// </summary>
public partial class PlayerVisualSync : AbstractComponent<Player>
{
	[Export] private Array<MeshInstance3D> modelsToColor;
	[Export] private Array<Node3D> elementsToHideAsFirstPerson;
	[Export] private Label3D nameTag;

	[Export] private Node3D logicalHand;
	[Export] private Node3D visualHand;

	[Export] private Node3D logicalNeck;
	[Export] private Node3D visualHead;
	[Export] private Node3D arms;
	[Export] private AnimationPlayer neckAnimator;
	[Export] private AnimationPlayer modelAnimator;
	[Export] private Dictionary<PlayerMovementType, string> movementTypeAnimation;



	public override void _EnterTree()
	{
		//If this player is the authority, the client's current profile will be sent to the server in order to request synchronization.
		if (IsMultiplayerAuthority())
		{
			UpdateProfile(PlayerProfileManager.Instance.CurrentProfile);
			foreach (Node3D element in elementsToHideAsFirstPerson)
			{
				element.Visible = false;
			}
		}
		if (Multiplayer.IsServer())
		{
			World.Instance.OnPlayerJoin += (id) =>
			{
				//When a player joins, the server will synchronize the data of the player who is hosting the game as well
				C2S_SyncProfile(ComponentParent.profileData.ToDictionary());
			};

		}
	}

	public override void _Process(double delta)
	{
		visualHead.Rotation = logicalNeck.Rotation;
		arms.Rotation = visualHead.Rotation;

		modelAnimator.Play(movementTypeAnimation[ComponentParent.playerMovement.MovementType]);
	}

	public void UpdateVisual()
	{
		//nametag text
		nameTag.Text = ComponentParent.profileData.PlayerName;
		nameTag.Modulate = ComponentParent.profileData.PlayerColor;
		nameTag.OutlineModulate = ColorUtils.InvertColor(ComponentParent.profileData.PlayerColor);
		foreach (MeshInstance3D element in modelsToColor)
		{
			ColorUtils.ChangeMeshColor(element, ComponentParent.profileData.PlayerColor);
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
			ComponentParent.profileData = newProfile;
		}
		UpdateVisual();

		//requests server for updating
		RpcId(1, nameof(C2S_SyncProfile), newProfile.ToDictionary());
	}

	//Client requests server to synchronize its profile.
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_SyncProfile(Dictionary profileDict)
	{
		PlayerProfileData receivedProfileData = PlayerProfileData.FromDictionary(profileDict);
		GD.PrintRich("[color=green](SYNC)[/color] Synchronization of player profile data");
		string hexColor = receivedProfileData.PlayerColor.ToHtml();
		GD.PrintRich($"{receivedProfileData.PlayerName}:[color={hexColor}]{hexColor}[/color]");
		Rpc(nameof(S2C_SyncProfile), profileDict);
		ComponentParent.profileData = receivedProfileData;
		UpdateVisual();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void S2C_SyncProfile(Dictionary profileDict)
	{
		ComponentParent.profileData = PlayerProfileData.FromDictionary(profileDict);
		UpdateVisual();
	}
}