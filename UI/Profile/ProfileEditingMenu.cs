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
	[Export] private TextureRect backgroundPreview;
	[Export] private TextureRect playerFacePreview;
	[Export] private ColorPicker colorEdit;
	[Export] private Button saveButton;
	[Export] private ItemList playerFaceList;

	public void _on_save_and_return_btn_pressed()
	{
		UpdateProfileFromUI();
		GetTree().ChangeSceneToPacked(ScenesBank.Instance.mainMenuScene);
	}

	public override void _Ready()
	{
		FetchFacesFromRegistry();
		UpdateUiFromProfile();
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
		backgroundPreview.Modulate = colorEdit.Color;
		playerFacePreview.Texture = GetSelectedFaceTexture();
	}

	private void UpdateUiFromProfile()
	{
		var currentProfile = PlayerProfileManager.Instance.CurrentProfile;
		//name
		if (!nameEdit.IsEditing())
			nameEdit.Text = currentProfile.PlayerName;
		//color
		backgroundPreview.Modulate = currentProfile.PlayerColor;
		colorEdit.Color = currentProfile.PlayerColor;
		//texture
		playerFacePreview.Texture = PlayerFaceRegistryManager.GetTextureByFaceId(currentProfile.PlayerFaceId);
	}

	private void UpdateProfileFromUI()
	{
		var currentProfile = PlayerProfileManager.Instance.CurrentProfile;

		currentProfile.PlayerName = nameEdit.Text;
		currentProfile.PlayerColor = colorEdit.Color;
		currentProfile.PlayerFaceId = GetSelectedFaceID();
		currentProfile.PrintProperties("Updated profile from UI");
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
