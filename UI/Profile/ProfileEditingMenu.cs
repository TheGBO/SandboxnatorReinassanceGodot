using Godot;
using Godot.Collections;
using NullCyan.Util;
using System;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Util.Log;
namespace NullCyan.Sandboxnator.UI;


public partial class ProfileEditingMenu : Control
{
	[Export] private LineEdit nameEdit;
	[Export] private TextureRect backgroundPreview;
	[Export] private TextureRect skinPreview;
	[Export] private ColorPicker colorEdit;
	[Export] private Button saveButton;

	public void _on_save_and_return_btn_pressed()
	{
		UpdateProfileFromUI();
		GetTree().ChangeSceneToPacked(ScenesBank.Instance.mainMenuScene);
	}

	public override void _Ready()
	{
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
	}

	private void UpdateUiFromProfile()
	{
		PlayerProfileData cpfd = PlayerProfileManager.Instance.CurrentProfile;
		//name
		if (!nameEdit.IsEditing())
			nameEdit.Text = cpfd.PlayerName;
		//color
		backgroundPreview.Modulate = cpfd.PlayerColor;
		colorEdit.Color = cpfd.PlayerColor;
	}

	private void UpdateProfileFromUI()
	{
		PlayerProfileManager.Instance.CurrentProfile.PlayerName = nameEdit.Text;
		PlayerProfileManager.Instance.CurrentProfile.PlayerColor = colorEdit.Color;
	}

	//Placeholder to test name generation, this random name generation should only happen when there is no existing player profile.
	

}
