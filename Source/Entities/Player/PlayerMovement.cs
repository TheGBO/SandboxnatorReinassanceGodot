using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Util.ComponentSystem;
using System;
namespace NullGarel.Sandboxnator.Entity;

[GodotClassName("PlayerMovement")]
public partial class PlayerMovement : AbstractComponent<Player>, ISettingsLoader
{
	//movement
	[ExportCategory("Components")]
	[Export] private CharacterBody3D _cbody;
	private float _currentSpeed;
	[ExportCategory("Movement parameters")]
	[Export] public float walkSpeed;
	[Export] public float sprintSpeed;
	[Export] public float jumpVelocity;
	private Vector3 _velocity;
	private bool _isMoving;
	private bool _isSprinting;
	public float HorizontalSpeed
	{
		get
		{
			return new Vector3(_velocity.X, 0, _velocity.Z).Length();
		}
	}
	//rigid body interaction
	// [Export] public float mass = 5f;
	// [Export] public float pushForceScalar = 2f;

	//visual effects
	[ExportCategory("Visual effects")]
	[Export] public Camera3D camera;
	[Export] public float sprintEffectTime = 0.75f;
	private float _fov = 75;
	/// <summary>
	/// Used for detection of movement and animations.
	/// </summary>
	[Export(PropertyHint.Enum, "FOR SYNCING PURPOSES!!!")]
	public PlayerMovementType MovementType { get; private set; }

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
		if (_cbody == null) return;
		if (!ComponentParent.IsMultiplayerAuthority()) return;
		
		camera.Fov = (float)GameRegistries.Instance.SettingsData.FieldOfView;
		SoundEffectProcess();
		MovementProcess(delta);
	}

	private void MovementProcess(double delta)
	{
		_velocity = _cbody.Velocity;

		// Add the gravity.
		if (!_cbody.IsOnFloor())
		{
			_velocity += _cbody.GetGravity() * (float)delta;
		}

		if (_cbody.IsOnFloor() && ComponentParent.playerInput.IsJumping)
		{
			_velocity.Y = jumpVelocity;
		}

		Vector3 forward = _cbody.GlobalTransform.Basis.Z;
		Vector3 right = _cbody.GlobalTransform.Basis.X;

		Vector2 inputDir = ComponentParent.playerInput.MovementVector;
		Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();
		_isMoving = inputDir != Vector2.Zero;

		//check for sprint
		_isSprinting = ComponentParent.playerInput.IsSprinting;
		if (_isSprinting)
		{
			MovementType = PlayerMovementType.Sprint;
			Sprint(true);
		}
		if (_isMoving && !_isSprinting)
		{
			MovementType = PlayerMovementType.Walk;
		}
		if (!_isMoving && !_isSprinting)
		{
			MovementType = PlayerMovementType.Idle;
		}

		if (direction != Vector3.Zero)
		{
			_velocity.X = direction.X * _currentSpeed;
			_velocity.Z = direction.Z * _currentSpeed;
		}
		else
		{
			_velocity.X = Mathf.MoveToward(_cbody.Velocity.X, 0, _currentSpeed);
			_velocity.Z = Mathf.MoveToward(_cbody.Velocity.Z, 0, _currentSpeed);
		}

		_cbody.Velocity = _velocity;
		_cbody.MoveAndSlide();
	}

	private void SoundEffectProcess()
	{
		if (_isMoving && _cbody.IsOnFloor())
		{
			float footstepDelay = _isSprinting ? 0.1f : 0.25f;
			ComponentParent.playerSounds.PlayGenericFootstep(footstepDelay);
		}
	}

	//not my code, adapted version from https://www.youtube.com/watch?v=Uh9PSOORMmA
	//DEPRECATED Disabled due to network issues.
	// private void PushAwayRigidBodies()
	// {
	// 	for (int i = 0; i < movementCBody.GetSlideCollisionCount(); i++)
	// 	{
	// 		KinematicCollision3D CollisionData = movementCBody.GetSlideCollision(i);

	// 		GodotObject UnkObj = CollisionData.GetCollider();

	// 		if (UnkObj is RigidBody3D)
	// 		{
	// 			RigidBody3D Obj = UnkObj as RigidBody3D;
	// 			float MassRatio = Mathf.Min(1.0f, mass / Obj.Mass);
	// 			if (MassRatio < 0.25f) continue;
	// 			Vector3 PushDir = -CollisionData.GetNormal();
	// 			float VelocityDiffInPushDir = movementCBody.Velocity.Dot(PushDir) - Obj.LinearVelocity.Dot(PushDir);
	// 			VelocityDiffInPushDir = Mathf.Max(0.0f, VelocityDiffInPushDir);
	// 			PushDir.Y = 0;
	// 			float PushForce = MassRatio * pushForceScalar;
	// 			Obj.ApplyImpulse(PushDir * VelocityDiffInPushDir * PushForce, CollisionData.GetPosition() - Obj.GlobalPosition);
	// 		}
	// 	}
	// }

	//input related.
	private void StopSprint()
	{
		Sprint(false);
	}

	private void Sprint(bool beginSprint)
	{
		MovementType = PlayerMovementType.Sprint;
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

	public void UpdateSettingsData()
	{
		_fov = (float)GameRegistries.Instance.SettingsData.FieldOfView;
	}
}
