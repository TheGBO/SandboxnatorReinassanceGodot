using Godot;
using Godot.Collections;
using NullCyan.Sandboxnator.Network;
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
		// If this player is the authority, send its profile to the server and hide first-person elements
		if (IsMultiplayerAuthority())
		{
			UpdateProfile(PlayerProfileManager.Instance.CurrentProfile);

			foreach (Node3D element in elementsToHideAsFirstPerson)
				element.Visible = false;
		}

		if (Multiplayer.IsServer() && World.HasInstance())
			World.Instance.OnPlayerJoin += OnPlayerJoinHandler;
	}
	
	private void OnPlayerJoinHandler(long id)
	{
		if (!IsInstanceValid(this)) return;

		// Sync profile when new players join
		C2S_SyncProfile(ComponentParent.profileData.ToDictionary());
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(visualHead) || !IsInstanceValid(logicalNeck) || !IsInstanceValid(arms))
			return;

		visualHead.Rotation = logicalNeck.Rotation;
		arms.Rotation = visualHead.Rotation;

		if (IsInstanceValid(modelAnimator))
			modelAnimator.Play(movementTypeAnimation[ComponentParent.playerMovement.MovementType]);
	}


	public void UpdateVisual()
	{
		if (!IsInstanceValid(nameTag))
			return;

		nameTag.Text = ComponentParent.profileData.PlayerName;
		nameTag.Modulate = ComponentParent.profileData.PlayerColor;
		nameTag.OutlineModulate = ColorUtils.InvertColor(ComponentParent.profileData.PlayerColor);

		foreach (MeshInstance3D element in modelsToColor)
		{
			if (IsInstanceValid(element))
				ColorUtils.ChangeMeshColor(element, ComponentParent.profileData.PlayerColor);
		}
	}

	/// <summary>
	/// Updates the player profile and visuals based on newProfile in the client side
	/// </summary>
	/// <param name="newProfile"></param>
	public void UpdateProfile(PlayerProfileData newProfile)
	{
		//In case of disconnections and reconnections to avoid referencing objects that are no longer in the world.
		if (!IsInstanceValid(this)) return;

		if (IsMultiplayerAuthority())
			ComponentParent.profileData = newProfile;

		UpdateVisual();

		if (Multiplayer.HasMultiplayerPeer() && IsInstanceValid(this))
			RpcId(1, nameof(C2S_SyncProfile), newProfile.ToDictionary());
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_SyncProfile(Dictionary profileDict)
	{
		if (!IsInstanceValid(this)) return;

		PlayerProfileData receivedProfileData = PlayerProfileData.FromDictionary(profileDict);

		GD.PrintRich("[color=green](SYNC)[/color] Synchronization of player profile data");
		string hexColor = receivedProfileData.PlayerColor.ToHtml();
		GD.PrintRich($"{receivedProfileData.PlayerName}:[color={hexColor}]{hexColor}[/color]");

		Rpc(nameof(S2C_SyncProfile), profileDict);

		ComponentParent.profileData = receivedProfileData;
		UpdateVisual();
	}

	//Client requests server to synchronize its profile.
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void S2C_SyncProfile(Dictionary profileDict)
	{
		if (!IsInstanceValid(this)) return;

		ComponentParent.profileData = PlayerProfileData.FromDictionary(profileDict);
		UpdateVisual();
	}
}

