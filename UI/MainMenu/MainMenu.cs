using Godot;
using System;
namespace NullGarel.Sandboxnator.UI;


public partial class MainMenu : Control
{
	[Export] public AcceptDialog notImplementedDialog;
	[Export] public ConfirmationDialog exitDialog;

	public override void _Ready()
	{
		UiSoundManager.Instance.TryInstallSounds();
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
