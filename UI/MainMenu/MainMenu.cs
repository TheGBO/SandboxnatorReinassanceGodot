using Godot;
using System;
namespace NullGarel.Sandboxnator.UI;


public partial class MainMenu : Control, IUiSignalLoader
{
	[ExportCategory("Dialog boxes")]
	[Export] private ConfirmationDialog _exitDialog;
	[ExportCategory("Main buttons")]
	[Export] private Button _playBtn;
	[Export] private Button _profileEditBtn;
	[Export] private Button _settingsBtn;
	[Export] private Button _exitBtn;


	public override void _Ready()
	{
		ConnectUISignals();
	}

	public void ConnectUISignals()
	{
		_playBtn.Pressed += SandboxnatorMain.Instance.ActivateWorldMenu;
		_settingsBtn.Pressed += SandboxnatorMain.Instance.ActivateSettingsMenu;
		_profileEditBtn.Pressed += SandboxnatorMain.Instance.ActivateProfileEditMenu;

		_exitBtn.Pressed += ExitDialogPrompt;
		_exitDialog.Confirmed += ExitDialogConfirmed;
	}


	public void DisconnectUISignals()
	{
		_playBtn.Pressed -= SandboxnatorMain.Instance.ActivateWorldMenu;
		_settingsBtn.Pressed -= SandboxnatorMain.Instance.ActivateSettingsMenu;
		_profileEditBtn.Pressed -= SandboxnatorMain.Instance.ActivateProfileEditMenu;

		_exitBtn.Pressed -= ExitDialogPrompt;
		_exitDialog.Confirmed -= ExitDialogConfirmed;
	}

	public void ExitDialogConfirmed()
	{
		ApplicationManager.Instance.HandleCloseRequest();
	}

	private void ExitDialogPrompt()
	{
		_exitDialog.Popup();
		UiSoundManager.Instance.PlaySfxSound(UiSoundType.PopUp);
	}
}
