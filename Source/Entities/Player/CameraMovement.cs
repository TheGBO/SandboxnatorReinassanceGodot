using Godot;
using System;
using NullCyan.Util.ComponentSystem;
namespace NullCyan.Sandboxnator.Entity;

public partial class CameraMovement : AbstractComponent<Player>
{
	[Export] public Node3D neck;
	[Export] public Node3D body;
	//TODO: move sensitivity to control settings
	float sensitivity = 1.0f/100.0f;
	public override void _Ready()
	{
		if (!ComponentParent.IsMultiplayerAuthority())
			return;

		Input.MouseMode = Input.MouseModeEnum.Captured;
		ComponentParent.playerInput.OnToggleCursorCapture += ToggleCursorCapture;
		ComponentParent.playerInput.OnMouseMovement += LookAction;
	}


	private void LookAction()
	{
		body.RotateY(-ComponentParent.playerInput.LookVector.X * sensitivity);
		neck.RotateX(-ComponentParent.playerInput.LookVector.Y * sensitivity);
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
