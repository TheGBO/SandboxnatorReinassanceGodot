using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] private PackedScene worldScene;
	[Export] private PackedScene profileScene;
	[Export] private AcceptDialog notImplementedDialog;
	[Export] private ConfirmationDialog exitDialog;

	public override void _Ready()
	{
		UiSoundManager.Instance.TryInstallSounds();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_play_btn_pressed()
	{
		GetTree().ChangeSceneToPacked(worldScene);
	}

	public void _on_customization_btn_pressed()
	{
		GetTree().ChangeSceneToPacked(profileScene);
	}

	public void _on_settings_btn_pressed()
	{
		ShowNotImplentedPopup();

	}

	public void _on_exit_btn_pressed()
	{
		exitDialog.Popup();
		UiSoundManager.Instance.PlaySfxSound(UiSoundType.PopUp);
	}

	public void _on_exit_dialog_confirmed()
	{
		GetTree().Quit();
	}

	private void ShowNotImplentedPopup()
	{
		UiSoundManager.Instance.PlaySfxSound(UiSoundType.PopUp);
		notImplementedDialog.Popup();
	}
}
