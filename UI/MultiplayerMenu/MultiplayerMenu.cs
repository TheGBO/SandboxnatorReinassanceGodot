using Godot;
using NullGarel.Sandboxnator.Network;
using System;
namespace NullGarel.Sandboxnator.UI;

public partial class MultiplayerMenu : Control
{
	[Export] private SpinBox portInput;
	[Export] private LineEdit ipAddressInput;
	[Export] private Control connectionMenu;
	[Export] private Control loadingScreen;
	[Export] private ProgressBar timeoutProgress;
	[Export] private Button cancelConnectionBtn;

	public override void _Ready()
	{
		UiSoundManager.Instance.TryInstallSounds();
		timeoutProgress.MaxValue = NetworkManager.ConnectionTimeoutLimit;
		cancelConnectionBtn.Pressed += () =>
		{
			NetworkManager.Instance.QuitConnection();
		};
	}

	public override void _Process(double delta)
	{
		timeoutProgress.Value = NetworkManager.ConnectionTimeoutLimit - NetworkManager.Instance.ElapsedConnectionTime;
		loadingScreen.Visible = NetworkManager.Instance.IsConnecting;
		bool showMenu = true;

		if (NetworkManager.Instance.peer != null)
		{
			MultiplayerPeer.ConnectionStatus status = NetworkManager.Instance.peer.GetConnectionStatus();
			showMenu = status != MultiplayerPeer.ConnectionStatus.Connected &&
					status != MultiplayerPeer.ConnectionStatus.Connecting;
		}

		connectionMenu.Visible = showMenu;
		//TODO: add timeout message
	}

	public void _on_host_btn_pressed()
	{
		NetworkManager.Instance.HostGame((int)portInput.Value, false);

	}

	public void _on_join_btn_pressed()
	{
		NetworkManager.Instance.JoinGame((int)portInput.Value, ipAddressInput.Text);
	}

	public void _on_main_menu_btn_pressed()
	{

		GetTree().ChangeSceneToPacked(ScenesBank.Instance.mainMenuScene);
	}

}
