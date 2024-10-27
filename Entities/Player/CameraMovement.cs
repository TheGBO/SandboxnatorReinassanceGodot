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
		parent.playerInput.OnMouseMovement += LookAction;
	}


	//TODO: (?) consider moving sensitivity to control settings
	private void LookAction()
	{
		body.RotateY(-parent.playerInput.LookVector.X * sensitivity);
		neck.RotateX(-parent.playerInput.LookVector.Y * sensitivity);
		neck.Rotation = new Vector3(Mathf.Clamp(neck.Rotation.X, -90 * (Mathf.Pi / 180), 90 * (Mathf.Pi / 180)), neck.Rotation.Y, neck.Rotation.Z);

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


}
