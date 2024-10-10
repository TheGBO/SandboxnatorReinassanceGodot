using Godot;
using System;

public partial class MultiplayerMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Visible = NetworkManager.Instance.peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected;
	}

	public void _on_host_btn_pressed()
	{
		GD.Print("HOST");
		NetworkManager.Instance.HostGame();
	}

	public void _on_join_btn_pressed()
	{
		NetworkManager.Instance.JoinGame();
	}


}
