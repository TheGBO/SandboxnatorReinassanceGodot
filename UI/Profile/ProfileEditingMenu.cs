using Godot;
using Godot.Collections;
using System;

public partial class ProfileEditingMenu : Control
{
	[Export] LineEdit nameEdit;
	[Export] TextureRect skinPreview;
	[Export] ColorPicker colorEdit;

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
		UpdateProfileFromUI();
		UpdateUiFromProfile();
	}

	public void _on_item_list_item_selected(int index)
	{
		GD.Print(index);
	}

	public void _on_color_picker_color_changed(Color color)
	{
		OnAlteration();
	}

	private void UpdateUiFromProfile()
	{
		PlayerProfileData cpfd = PlayerProfileManager.Instance.CurrentProfile;
		//name
		nameEdit.Text = cpfd.PlayerName;
		//color
		skinPreview.Modulate = cpfd.PlayerColor;
		colorEdit.Color = cpfd.PlayerColor;
	}

	private void UpdateProfileFromUI()
	{
		PlayerProfileManager.Instance.CurrentProfile.PlayerName = nameEdit.Text;
		PlayerProfileManager.Instance.CurrentProfile.PlayerColor = colorEdit.Color;
	}

	//Placeholder to test name generation, this random name generation should only happen when there is no existing player profile.
	private void FillNameField()
	{
		NameGenerator nameGen = NameGenerator.Create();
		GD.Randomize();
		if (GD.Randf() >= 0.5)
		{
			nameGen.UseWesternPatterns();
		}
		else
		{
			nameGen.UseSimplePatterns();
		}
		string name = nameGen.GenerateName();
		string nameCorrected = char.ToUpper(name[0]) + name.Substring(1);
		nameEdit.Text = nameCorrected;
		GD.Print(nameCorrected);
	}

}
