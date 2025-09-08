using Godot;
using NullCyan.Sandboxnator.Network;
using System;
namespace NullCyan.Sandboxnator.UI;

public partial class MultiplayerMenu : Control
{
	[Export] private SpinBox portInput;
	[Export] private LineEdit ipAddressInput;

	public override void _Ready()
	{
		UiSoundManager.Instance.TryInstallSounds();
	}

	public override void _Process(double delta)
	{
		Visible = NetworkManager.Instance?.peer?.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Connected;
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
