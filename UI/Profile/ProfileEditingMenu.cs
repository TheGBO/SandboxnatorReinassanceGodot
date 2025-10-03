using Godot;
using Godot.Collections;
using NullCyan.Util;
using System;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Util.Log;
using NullCyan.Util.GodotHelpers;
using NullCyan.Sandboxnator.Item;
using NullCyan.Sandboxnator.Entity.PlayerCosmetics;
using NullCyan.Sandboxnator.Registry;
namespace NullCyan.Sandboxnator.UI;


public partial class ProfileEditingMenu : Control
{
	[Export] private LineEdit nameEdit;
	[Export] private PlayerModel playerModelPreview;
	[Export] private Camera3D previewCamera;
	[Export] private ColorPicker colorEdit;
	[Export] private Button saveButton;
	[Export] private ItemList playerFaceList;
	private PlayerProfileData _cachedProfile = new();

	public void _on_save_and_return_btn_pressed()
	{
		UpdateProfileFromUI();
		GetTree().ChangeSceneToPacked(ScenesBank.Instance.mainMenuScene);
	}

	public override void _Ready()
	{
		_cachedProfile = PlayerProfileManager.Instance.CurrentProfile;
		FetchFacesFromRegistry();
		UpdateUiFromProfile();
	}

	public override void _Process(double delta)
	{
		if (PlatformCheck.IsDesktop())
		{
			Viewport viewPort = GetViewport();
			Vector2 mousePos = viewPort.GetMousePosition();
			float visibleRect = viewPort.GetVisibleRect().Size.X;
			float mouseOnScreenRatio = (mousePos.X / visibleRect) - 0.5f;
			playerModelPreview.GlobalRotation = new(0, mouseOnScreenRatio * Mathf.Pi - Mathf.Pi, 0);
			GD.Print(mouseOnScreenRatio);

		}

	}


	public void OnAlteration()
	{
		UpdateUiFromUi();
	}

	#region ALTERATION HANDLERs
	public void _on_item_list_item_selected(int index)
	{
		OnAlteration();
	}

	public void _on_color_picker_color_changed(Color color)
	{
		OnAlteration();
	}

	public void _on_name_edit_text_changed(string newName)
	{
		OnAlteration();
	}
	#endregion

	private bool ValidateProfile()
	{
		bool isNameValid = !nameEdit.Text.Contains('!') && !string.IsNullOrEmpty(nameEdit.Text);
		saveButton.Disabled = !isNameValid;
		bool validProfile = isNameValid;
		return validProfile;
	}

	private void UpdateUiFromUi()
	{
		_cachedProfile.PlayerName = nameEdit.Text;
		_cachedProfile.PlayerColor = colorEdit.Color;
		_cachedProfile.PlayerFaceId = GetSelectedFaceID();
		playerModelPreview.UpdateVisual(_cachedProfile);
	}

	private void UpdateUiFromProfile()
	{
		//name
		if (!nameEdit.IsEditing())
			nameEdit.Text = _cachedProfile.PlayerName;
		//color
		colorEdit.Color = _cachedProfile.PlayerColor;
		playerModelPreview.UpdateVisual(_cachedProfile);
	}

	private void UpdateProfileFromUI()
	{
		PlayerProfileManager.Instance.CurrentProfile = _cachedProfile;
		_cachedProfile.PrintProperties("Updated profile from UI");
	}


	#region Face registry related
	private void FetchFacesFromRegistry()
	{
		playerFaceList.Clear();
		foreach (PlayerFaceData face in GameRegistries.Instance.PlayerFaceRegistry.GetAllValues())
		{
			NcLogger.Log($"Found a face: id={face.playerFaceId}");
			playerFaceList.AddItem(face.playerFaceId, face.faceTexture);

		}
	}

	private Texture2D GetSelectedFaceTexture() => PlayerFaceRegistryManager.GetTextureByFaceId(GetSelectedFaceID());

	private string GetSelectedFaceID()
	{
		var currentProfile = PlayerProfileManager.Instance.CurrentProfile;
		var selected = playerFaceList.GetSelectedItems();
		if (selected.Length > 0)
		{
			int index = selected[0];
			string faceID = playerFaceList.GetItemText(index);
			GD.Print("Selected item name: " + faceID);
			return faceID;
		}
		return currentProfile.PlayerFaceId;
	}
	#endregion
	//Placeholder to test name generation, this random name generation should only happen when there is no existing player profile.


}
