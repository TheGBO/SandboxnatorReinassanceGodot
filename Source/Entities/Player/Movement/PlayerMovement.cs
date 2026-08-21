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
	Sprint,
	Jump,
	Fall
}

[GodotClassName("PlayerMovement")]
public partial class PlayerMovement : AbstractComponent<Player>
{
	//movement
	[ExportCategory("Nodes")]
	[Export] private CharacterBody3D _characterBody;
	[ExportCategory("Movement parameters")]
	[Export] public float walkSpeed;
	[Export] public float sprintSpeed;
	[Export] public float jumpVelocity;
	//state trackers
	private Vector3 _velocity;
	private float _currentSpeed;

	public float HorizontalSpeed
	{
		get
		{
			return new Vector3(_velocity.X, 0, _velocity.Z).Length();
		}
	}

	[Export(PropertyHint.Enum, "Do not alter it in the editor. This is used for animations and for the internal FSM.")]
	public MovementState MovementType { get; private set; }

	private PlayerInput _playerInput;

	public override void _Ready()
	{
		if (!ComponentParent.IsMultiplayerAuthority())
			return;

		_playerInput = GetComponent<PlayerInput>();

		_currentSpeed = walkSpeed;
		MovementType = MovementState.Idle;
		_playerInput.OnStopSprint += StopSprint;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!Multiplayer.HasMultiplayerPeer()) return;
		if (_characterBody == null) return;
		if (!ComponentParent.IsMultiplayerAuthority()) return;

		MovementProcess(delta);
	}

	private void MovementProcess(double delta)
	{
		_velocity = _characterBody.Velocity;

		Vector3 forward = _characterBody.GlobalTransform.Basis.Z;
		Vector3 right = _characterBody.GlobalTransform.Basis.X;

		Vector2 inputDir = _playerInput.MovementVector;
		Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();

		DecideNextState(inputDir);
		RunStateBehavior(direction, delta);

		_characterBody.Velocity = _velocity;
		_characterBody.MoveAndSlide();
	}

	/// <summary>
	/// Just so I dn't forget the priority:
	/// 1 air 
	/// 2 grounded input
	/// 3 jump-press 
	/// 4 walking 
	/// 5 input 
	/// 6 idle.
	/// </summary>
	private void DecideNextState(Vector2 inputDir)
	{
		bool onFloor = _characterBody.IsOnFloor();

		if (!onFloor)
		{
			// Airborne states
			if (MovementType != MovementState.Jump && MovementType != MovementState.Fall)
				SwitchToState(MovementState.Fall);
			return;
		}

		if (_playerInput.IsJumping)
		{
			SwitchToState(MovementState.Jump);
			return;
		}

		if (inputDir != Vector2.Zero)
		{
			SwitchToState(_playerInput.IsSprinting ? MovementState.Sprint : MovementState.Walk);
			return;
		}

		SwitchToState(MovementState.Idle);
	}

	private void RunStateBehavior(Vector3 direction, double delta)
	{
		//call this on states that account for horizontal movement.
		void InternalHorizontalInput()
		{
			_velocity.X = direction.X * _currentSpeed;
			_velocity.Z = direction.Z * _currentSpeed;
		}

		switch (MovementType)
		{
			case MovementState.Fall:
				_velocity += _characterBody.GetGravity() * (float)delta;
				InternalHorizontalInput();
				break;

			case MovementState.Jump:
				//impulse, then fall.
				_velocity.Y = jumpVelocity;
				InternalHorizontalInput();
				SwitchToState(MovementState.Fall);
				break;

			case MovementState.Sprint:
				InternalHorizontalInput();
				break;

			case MovementState.Walk:
				InternalHorizontalInput();
				break;

			case MovementState.Idle:
				_velocity.X = Mathf.MoveToward(_velocity.X, 0, _currentSpeed);
				_velocity.Z = Mathf.MoveToward(_velocity.Z, 0, _currentSpeed);
				break;
		}
	}

	private void SwitchToState(MovementState newState)
	{
		if (newState == MovementType) return;

		OnExitState(MovementType);
		MovementType = newState;
		OnEnterState(newState);
	}

	private void OnEnterState(MovementState state)
	{
		if (state == MovementState.Sprint)
		{
			//TODO: Invoke event to alert interested components for animation purposes
		}

	}

	private void OnExitState(MovementState state)
	{
		if (state == MovementState.Sprint)
		{
			//TODO: Invoke event to alert interested components for animation purposes

		}
	}

	private void StopSprint()
	{
		if (MovementType == MovementState.Sprint)
			SwitchToState(MovementState.Walk);
	}

}