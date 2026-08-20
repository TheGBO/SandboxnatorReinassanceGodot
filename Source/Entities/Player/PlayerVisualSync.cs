using Godot;
using Godot.Collections;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.GodotHelpers;

namespace NullGarel.Sandboxnator.Entity;

/// <summary>
/// A component responsible for the cosmetics, animations and profile synchronization.
/// </summary>
public partial class PlayerVisualSync : AbstractComponent<Player>
{
	[ExportCategory("In-Game")]
	[Export] private Array<Node3D> _elementsToHideAsFirstPerson;

	[ExportCategory("Player model")]
	[Export] private PlayerModel _playerModel;
	[Export] public Label3D _nameTag;
	[Export] private Array<Node3D> _rotateAlongNeck;
	[Export] private Node3D _neck;

	[ExportCategory("Animations")]
	[Export] private AnimationPlayer _movementStateAnimation;
	private const string IdleAnimation = "IdleAndHold";
	private const string WalkAnimation = "WalkAndHold";

	//Serialization
	private PlayerProfileData _profileData = new();
	private Dictionary _profileDataDict;

	//components
	private PlayerMovement _playerMovement;

	/// <summary>
	/// This dictionary is directly synced by <see cref="MultiplayerSynchronizer"/>
	/// </summary>
	[Export]
	public Dictionary ProfileDataDict
	{
		get => _profileDataDict;
		set
		{
			_profileDataDict = value;
			if (value != null && value.Count > 0)
			{
				_profileData = DictPack.Deserialize<PlayerProfileData>(value);
				ApplyProfile(_profileData);
			}
		}
	}

	public override void _EnterTree()
	{
		if (IsMultiplayerAuthority())
		{
			foreach (Node3D element in _elementsToHideAsFirstPerson)
				element.Visible = false;
			ProfileDataDict = DictPack.Serialize(PlayerProfileManager.Instance.CurrentProfile);
			_playerModel.handMesh.SetMeshClip(true);
		}
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		_playerMovement = GetComponent<PlayerMovement>();
	}

	public override void _Process(double delta)
	{
		foreach (Node3D target in _rotateAlongNeck)
		{
			target.GlobalRotation = _neck.GlobalRotation;
		}
		switch (_playerMovement.MovementType)
		{
			case MovementState.Idle:
				_movementStateAnimation.CurrentAnimation = IdleAnimation;
				break;

			case MovementState.Walk:
				_movementStateAnimation.CurrentAnimation = WalkAnimation;

				break;
			case MovementState.Sprint:
				_movementStateAnimation.CurrentAnimation = WalkAnimation;
				break;
		}
	}

	/// <summary>
	/// This method is responsible for turning the profile data into visual information in-game.
	/// </summary>
	/// <param name="profile"></param>
	public void ApplyProfile(PlayerProfileData profile)
	{
		if (profile == null) return;

		if (ComponentParent != null)
		{
			ComponentParent.ProfileData = _profileData;
		}

		if (IsInstanceValid(_nameTag))
		{
			_nameTag.Text = profile.PlayerName;
			_nameTag.Modulate = profile.PlayerColor;
			_nameTag.OutlineModulate = ColorAndMeshUtils.InvertColor(profile.PlayerColor);
		}

		if (IsInstanceValid(_playerModel))
		{
			_playerModel.UpdateVisual(profile);
		}
	}

}
