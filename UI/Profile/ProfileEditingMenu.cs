using Godot;
using System;
using NullGarel.Sandboxnator.Entity;
using NullGarel.Util.GodotHelpers;
using NullGarel.Sandboxnator.Entity.PlayerCosmetics;
using NullGarel.Sandboxnator.Registry;
namespace NullGarel.Sandboxnator.UI;


public partial class ProfileEditingMenu : Control, IUiSignalLoader
{
	[ExportCategory("Character customization")]
	[Export] private LineEdit _nameEdit;
	[Export] private ColorPicker _colorEdit;
	[Export] private ItemList _playerFaceList;
	[ExportCategory("Preview")]
	[Export] private Camera3D _previewCamera;
	[Export] private PlayerModel _playerModelPreview;
	private CanvasLayer _parentCanvasLayer;
	[ExportCategory("Main UI")]
	[Export] private Button _saveButton;
	private PlayerProfileData _cachedProfile = new();


	public override void _Ready()
	{
		//assuming this is supposed to never fail (because it is lol)
		_parentCanvasLayer = GetParent<CanvasLayer>();

		ConnectUISignals();
		_cachedProfile = PlayerProfileManager.Instance.CurrentProfile;
		FetchFacesFromRegistry();
		UpdateUiFromCachedProfile();
		_parentCanvasLayer.VisibilityChanged += UpdateVisibility;
		UpdateVisibility();
	}

	public void ConnectUISignals()
	{
		_saveButton.Pressed += () =>
		{
			ConfirmProfile();
			SandboxnatorMain.Instance.ActivateMainMenu();
		};
		_nameEdit.TextChanged += (_) => OnAlteration();
		_playerFaceList.ItemSelected += (_) => OnAlteration();
		_colorEdit.ColorChanged += (_) => OnAlteration();
	}

	public override void _Process(double delta)
	{
		ModelLookAtCursor();
	}

	private void UpdateVisibility()
	{
		Visible = _parentCanvasLayer.Visible;
		_playerModelPreview.Visible = Visible;
	}

	private void ModelLookAtCursor()
	{
		if (!PlatformCheck.IsDesktop()) return;

		Viewport viewPort = GetViewport();
		Vector2 mousePos = viewPort.GetMousePosition();
		float visibleRect = viewPort.GetVisibleRect().Size.X;
		float mouseOnScreenRatio = (mousePos.X / visibleRect) - 0.5f;
		_playerModelPreview.GlobalRotation = new(0, mouseOnScreenRatio * Mathf.Pi - Mathf.Pi, 0);

	}


	public void OnAlteration()
	{
		UpdateCachedProfileFromUi();
	}

	private bool ValidateProfile()
	{
		bool isNameValid = !_nameEdit.Text.Contains('!') && !string.IsNullOrEmpty(_nameEdit.Text);
		_saveButton.Disabled = !isNameValid;
		bool validProfile = isNameValid;
		return validProfile;
	}

	private void UpdateCachedProfileFromUi()
	{
		_cachedProfile.PlayerName = _nameEdit.Text;
		_cachedProfile.PlayerColor = _colorEdit.Color;
		_cachedProfile.PlayerFaceId = GetSelectedFaceID();
		_playerModelPreview.UpdateVisual(_cachedProfile);
	}

	private void UpdateUiFromCachedProfile()
	{
		//name
		if (!_nameEdit.IsEditing())
			_nameEdit.Text = _cachedProfile.PlayerName;
		//color
		_colorEdit.Color = _cachedProfile.PlayerColor;
		_playerModelPreview.UpdateVisual(_cachedProfile);
	}

	private void ConfirmProfile()
	{
		PlayerProfileManager.Instance.CurrentProfile = _cachedProfile;
		_cachedProfile.PrintProperties("Updated profile from UI");
	}


	#region Face registry related
	private void FetchFacesFromRegistry()
	{
		_playerFaceList.Clear();
		foreach (PlayerFaceData face in GameRegistries.Instance.PlayerFaceRegistry.GetAllValues())
		{
			_playerFaceList.AddItem(face.PlayerFaceId, face.FaceTexture);
		}
	}

	private Texture2D GetSelectedFaceTexture() => PlayerFaceRegistryManager.GetTextureByFaceId(GetSelectedFaceID());

	private string GetSelectedFaceID()
	{
		PlayerProfileData currentProfile = PlayerProfileManager.Instance.CurrentProfile;
		int[] selected = _playerFaceList.GetSelectedItems();
		if (selected.Length > 0)
		{
			int index = selected[0];
			string faceID = _playerFaceList.GetItemText(index);
			return faceID;
		}
		return currentProfile.PlayerFaceId;
	}

	public void DisconnectUISignals()
	{
		throw new NotImplementedException();
	}

	#endregion

}
