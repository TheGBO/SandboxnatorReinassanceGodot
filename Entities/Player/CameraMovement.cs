using Godot;
using System;

public partial class CameraMovement : AbstractPlayerComponent
{
	[Export] public Node3D neck;
	[Export] public Node3D body;
	float sensitivity = 0.01f;
	public override void _Ready()
	{
		if (!parent.IsMultiplayerAuthority())
			return;

		Input.MouseMode = Input.MouseModeEnum.Captured;
		parent.playerInput.OnToggleCursorCapture += ToggleCursorCapture;
	}

	private void ToggleCursorCapture()
	{
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	//TODO: Move to PlayerInput
	public override void _Input(InputEvent _event)
	{
		if (parent.IsMultiplayerAuthority())
		{
			if (parent == null)
			{
				GD.Print("NULL PARENT WARNING");
			}
			if (_event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				body.RotateY(-mouseMotion.Relative.X * sensitivity);
				neck.RotateX(-mouseMotion.Relative.Y * sensitivity);
				neck.Rotation = new Vector3(Mathf.Clamp(neck.Rotation.X, -90 * (Mathf.Pi / 180), 90 * (Mathf.Pi / 180)), neck.Rotation.Y, neck.Rotation.Z);

			}
		}
	}
}
