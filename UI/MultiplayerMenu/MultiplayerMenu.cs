using Godot;
using System;

public partial class MultiplayerMenu : Control
{

	public override void _Ready()
	{
		UiSoundManager.Instance.TryInstallSounds();
	}

	public override void _Process(double delta)
	{
		Visible = NetworkManager.Instance.peer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Connected;

	}

	public void _on_host_btn_pressed()
	{
		NetworkManager.Instance.HostGame();

	}

	public void _on_join_btn_pressed()
	{
		NetworkManager.Instance.JoinGame();
	}

}
