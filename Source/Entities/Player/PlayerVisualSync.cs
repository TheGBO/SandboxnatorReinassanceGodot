using Godot;
using Godot.Collections;
using NullCyan.Util.ComponentSystem;
using NullCyan.Util.GodotHelpers;

namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerVisualSync : AbstractComponent<Player>
{
	[ExportGroup("In-Game")]
	[Export] private Array<Node3D> elementsToHideAsFirstPerson;

	[ExportGroup("Player model")]
	[Export] private PlayerModel playerModel;
	[Export] public Label3D nameTag;

	//Serialization
	private PlayerProfileData _profileData = new();
	private Dictionary _profileDataDict;

	[Export]
	public Dictionary ProfileDataDict
	{
		get => _profileDataDict;
		set
		{
			_profileDataDict = value;
			if (value != null && value.Count > 0)
			{
				_profileData = PlayerProfileData.FromDictionary(value);
				ApplyProfile(_profileData);
			}
		}
	}



	public override void _EnterTree()
	{
		if (IsMultiplayerAuthority())
		{
			foreach (Node3D element in elementsToHideAsFirstPerson)
				element.Visible = false;
			ProfileDataDict = PlayerProfileManager.Instance.CurrentProfile.ToDictionary();
			ColorAndMeshUtils.SetMeshClip(playerModel.handMesh, true);
		}
	}

	public void ApplyProfile(PlayerProfileData profile)
	{
		if (profile == null) return;

		if (ComponentParent != null)
		{
			ComponentParent.ProfileData = _profileData;
		}

		if (IsInstanceValid(nameTag))
		{
			nameTag.Text = profile.PlayerName;
			nameTag.Modulate = profile.PlayerColor;
			nameTag.OutlineModulate = ColorAndMeshUtils.InvertColor(profile.PlayerColor);
		}

		if (IsInstanceValid(playerModel))
		{
			playerModel.UpdateVisual(profile);
		}
	}
}