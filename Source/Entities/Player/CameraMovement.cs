using Godot;
using System;
using NullGarel.Util.ComponentSystem;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
namespace NullGarel.Sandboxnator.Entity;

public partial class CameraMovement : AbstractComponent<Player>, ISettingsLoader
{
	private const float SENSITIVITY_DENOMINATOR = 10000.0f;

	[Export] public Node3D neck;
	[Export] public Node3D body;

	private float _sensitivity;
	private PlayerInput _playerInput;

	public override void _Ready()
	{

		if (!ComponentParent.IsMultiplayerAuthority())
			return;

		GameRegistries.Instance.OnSettingsChanged += UpdateSettingsData;
		UpdateSettingsData();

		_playerInput = ComponentParent.componentHolder.GetComponent<PlayerInput>();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_playerInput.OnToggleCursorCapture += ToggleCursorCapture;
		_playerInput.OnMouseMovement += LookAction;
	}

	private void LookAction()
	{
		body.RotateY(-_playerInput.LookVector.X * _sensitivity);
		neck.RotateX(-_playerInput.LookVector.Y * _sensitivity);
		neck.Rotation = new(Mathf.Clamp(neck.Rotation.X, -90 * (Mathf.Pi / 180), 90 * (Mathf.Pi / 180)), neck.Rotation.Y, neck.Rotation.Z);

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
		_sensitivity = (float)(GameRegistries.Instance.SettingsData.LookSensitivity / SENSITIVITY_DENOMINATOR);
	}
}
