using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] private PackedScene worldScene;

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
}
