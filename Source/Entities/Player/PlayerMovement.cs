using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Util.ComponentSystem;
using System;
namespace NullGarel.Sandboxnator.Entity;

public enum MovementState
{
	Idle,
	Walk,
	Sprint
}

[GodotClassName("PlayerMovement")]
public partial class PlayerMovement : AbstractComponent<Player>, ISettingsLoader
{
	//movement
	[ExportCategory("Nodes")]
	[Export] private CharacterBody3D _characterBody;
	[Export] public Camera3D camera;
	[ExportCategory("Movement parameters")]
	[Export] public float walkSpeed;
	[Export] public float sprintSpeed;
	[Export] public float jumpVelocity;
	//state trackers
	private Vector3 _velocity;
	private bool _isMoving;
	private bool _isSprinting;
	private float _currentSpeed;
	public float HorizontalSpeed
	{
		get
		{
			return new Vector3(_velocity.X, 0, _velocity.Z).Length();
		}
	}

	[ExportCategory("Visual effects")]
	[Export] public float sprintEffectTime = 0.75f;
	private float _fov = 75;
	[Export(PropertyHint.Enum, "Do not alter it in the editor. This is used for animations.")]
	public MovementState MovementType { get; private set; }

	public override void _Ready()
	{
		if (!ComponentParent.IsMultiplayerAuthority())
			return;

		UpdateSettingsData();

		_currentSpeed = walkSpeed;
		ComponentParent.playerInput.OnStopSprint += StopSprint;
	}


	public override void _PhysicsProcess(double delta)
	{
		if (!Multiplayer.HasMultiplayerPeer()) return;
		if (_characterBody == null) return;
		if (!ComponentParent.IsMultiplayerAuthority()) return;

		camera.Fov = (float)GameRegistries.Instance.SettingsData.FieldOfView;
		SoundEffectProcess();
		MovementProcess(delta);
	}

	private void MovementProcess(double delta)
	{
		_velocity = _characterBody.Velocity;

		// Add the gravity.
		if (!_characterBody.IsOnFloor())
		{
			_velocity += _characterBody.GetGravity() * (float)delta;
		}

		if (_characterBody.IsOnFloor() && ComponentParent.playerInput.IsJumping)
		{
			_velocity.Y = jumpVelocity;
		}

		Vector3 forward = _characterBody.GlobalTransform.Basis.Z;
		Vector3 right = _characterBody.GlobalTransform.Basis.X;

		Vector2 inputDir = ComponentParent.playerInput.MovementVector;
		Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();
		_isMoving = inputDir != Vector2.Zero;

		//check for sprint
		_isSprinting = ComponentParent.playerInput.IsSprinting;
		if (_isSprinting)
		{
			MovementType = MovementState.Sprint;
			Sprint(true);
		}
		if (_isMoving && !_isSprinting)
		{
			MovementType = MovementState.Walk;
		}
		if (!_isMoving && !_isSprinting)
		{
			MovementType = MovementState.Idle;
		}

		if (direction != Vector3.Zero)
		{
			_velocity.X = direction.X * _currentSpeed;
			_velocity.Z = direction.Z * _currentSpeed;
		}
		else
		{
			_velocity.X = Mathf.MoveToward(_characterBody.Velocity.X, 0, _currentSpeed);
			_velocity.Z = Mathf.MoveToward(_characterBody.Velocity.Z, 0, _currentSpeed);
		}

		_characterBody.Velocity = _velocity;
		_characterBody.MoveAndSlide();
	}

	private void SoundEffectProcess()
	{
		if (_isMoving && _characterBody.IsOnFloor())
		{
			float footstepDelay = _isSprinting ? 0.1f : 0.25f;
			ComponentParent.playerSounds.PlayGenericFootstep(footstepDelay);
		}
	}
	
	//input related.
	private void StopSprint()
	{
		Sprint(false);
	}

	private void Sprint(bool beginSprint)
	{
		MovementType = MovementState.Sprint;
		Tween sprintTween = GetTree().CreateTween();
		if (beginSprint)
		{
			sprintTween.TweenProperty(camera, "fov", _fov * 1.25, sprintEffectTime);
			sprintTween.TweenProperty(this, nameof(_currentSpeed), sprintSpeed, sprintEffectTime);
		}
		else
		{
			sprintTween.TweenProperty(camera, "fov", _fov, sprintEffectTime);
			sprintTween.TweenProperty(this, nameof(_currentSpeed), walkSpeed, sprintEffectTime);
		}
	}

	/// <summary>
	/// Load the FieldOfView property from the game settings.
	/// </summary>
	public void UpdateSettingsData()
	{
		_fov = (float)GameRegistries.Instance.SettingsData.FieldOfView;
	}
}
