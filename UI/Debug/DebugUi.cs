using Godot;

public partial class DebugUi : Control
{
	[Export] private Label _fpsLabel;

	public override void _Process(double delta)
	{
		_fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
	}
}
