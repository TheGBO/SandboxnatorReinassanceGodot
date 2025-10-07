using Godot;
using Godot.Collections;
using NullCyan.Sandboxnator.Network;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util.ComponentSystem;
using NullCyan.Util.IO;
using NullCyan.Sandboxnator.Entity.PlayerCosmetics;
namespace NullCyan.Sandboxnator.Entity;

/// <summary>
/// Handles the visual effects and their multiplayer synchronization.
/// Manages <see cref="PlayerModel"/> during in-world runtime
/// </summary>
public partial class PlayerVisualSync : AbstractComponent<Player>
{
	[ExportGroup("In-Game")]
	[Export] private Array<Node3D> elementsToHideAsFirstPerson;

	[ExportGroup("Positions and runtime nodes")]
	[Export] private Node3D logicalNeck;
	[Export] private Node3D itemHoldingPoint;
	[Export] private Node3D logicalHand;
	[Export] private Node3D headHolder;
	[Export] private Node3D handsHolder;
	[ExportGroup("Animations")]
	[Export] private Dictionary<PlayerMovementType, string> movementTypeAnimation;
	[Export] private AnimationPlayer neckAnimator;
	[Export] private AnimationPlayer modelAnimator;
	[ExportGroup("Player model")]
	[Export] private PlayerModel playerModel;
	//Currently, a nametag is a in-world characteristic due to the text editing available at ProfileEditingMenu
	[Export] public Label3D nameTag; 


	public override void _EnterTree()
	{
		// If this player is the authority, send its profile to the server and hide first-person elements
		if (IsMultiplayerAuthority())
		{
			UpdateProfile(PlayerProfileManager.Instance.CurrentProfile);

			foreach (Node3D element in elementsToHideAsFirstPerson)
				element.Visible = false;
			//use_z_clip_scale : Make hand not clip through objects as first person.
			//RUNTIME
			ColorAndMeshUtils.SetMeshClip(playerModel.handMesh, true);
		}

		if (Multiplayer.IsServer() && World.HasInstance())
			World.Instance.OnPlayerJoin += OnPlayerJoinHandler;
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(headHolder) || !IsInstanceValid(logicalNeck) || !IsInstanceValid(handsHolder))
			return;

		headHolder.Rotation = logicalNeck.Rotation;
		handsHolder.Rotation = headHolder.Rotation;

		if (IsInstanceValid(modelAnimator))
			modelAnimator.Play(movementTypeAnimation[ComponentParent.playerMovement.MovementType]);
	}

	#region SYNCs
	private void OnPlayerJoinHandler(long id)
	{
		if (!IsInstanceValid(this)) return;

		// Sync profile when new players join
		C2S_Sync(MPacker.Pack(ComponentParent.ProfileData));
	}

	public void UpdateVisual()
	{
		if (!IsInstanceValid(nameTag))
			return;
		//Nametag logic
		nameTag.Text = ComponentParent.ProfileData.PlayerName;
		nameTag.Modulate = ComponentParent.ProfileData.PlayerColor;
		nameTag.OutlineModulate = ColorAndMeshUtils.InvertColor(ComponentParent.ProfileData.PlayerColor);
		//Player model logic
		playerModel.UpdateVisual(ComponentParent.ProfileData);
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
			ComponentParent.ProfileData = newProfile;

		UpdateVisual();

		newProfile.PrintProperties("[CLIENT] Data before being packed and sent to the server");
		byte[] packedProfileData = MPacker.Pack(newProfile);
		MPacker.Unpack<PlayerProfileData>(packedProfileData).PrintProperties("[CLIENT] Data as it's being sent to server");

		if (Multiplayer.HasMultiplayerPeer() && IsInstanceValid(this))
			RpcId(1, nameof(C2S_Sync), packedProfileData);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void C2S_Sync(byte[] profileBytes)
	{
		if (!IsInstanceValid(this)) return;

		PlayerProfileData unpackedProfileData = MPacker.Unpack<PlayerProfileData>(profileBytes);
		unpackedProfileData.PrintProperties("[SERVER] received player profile data as");
		Rpc(nameof(S2C_Sync), profileBytes);

		ComponentParent.ProfileData = unpackedProfileData;
		UpdateVisual();
	}

	//Client requests server to synchronize its profile. This call sends the profile data to the client.
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void S2C_Sync(byte[] profileBytes)
	{
		if (!IsInstanceValid(this)) return;

		PlayerProfileData unpackedProfileData = MPacker.Unpack<PlayerProfileData>(profileBytes);
		ComponentParent.ProfileData = unpackedProfileData;
		unpackedProfileData.PrintProperties("[CLIENT] received profile data as");
		UpdateVisual();
	}

	#endregion

}
