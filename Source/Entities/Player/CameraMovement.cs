using Godot;
using System;
using NullCyan.Util.ComponentSystem;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Sandboxnator.Settings;
namespace NullCyan.Sandboxnator.Entity;

public partial class CameraMovement : AbstractComponent<Player>, ISettingsLoader
{
	[Export] public Node3D neck;
	[Export] public Node3D body;
	[Export] public Camera3D camera;

	private const float SENSITIVITY_DENOMINATOR = 10000.0f;
	float sensitivity;
	public override void _Ready()
	{
		if (!ComponentParent.IsMultiplayerAuthority())
			return;

		GD.Print($"[DEBUG] camera={camera}, GameRegistries.Instance={GameRegistries.Instance}, SettingsData={(GameRegistries.Instance != null ? GameRegistries.Instance.SettingsData : null)}");
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

	public void UpdateSettingsData()
	{
		camera.Fov = GameRegistries.Instance.SettingsData.FieldOfView;
		sensitivity = GameRegistries.Instance.SettingsData.Sensitivity / SENSITIVITY_DENOMINATOR;
	}
}
