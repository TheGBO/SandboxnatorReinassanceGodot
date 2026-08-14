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
	[ExportGroup("In-Game")]
	[Export] private Array<Node3D> _elementsToHideAsFirstPerson;

	[ExportGroup("Player model")]
	[Export] private PlayerModel _playerModel;
	[Export] public Label3D _nameTag;

	//Serialization
	private PlayerProfileData _profileData = new();
	private Dictionary _profileDataDict;

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
			ColorAndMeshUtils.SetMeshClip(_playerModel.handMesh, true);
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
