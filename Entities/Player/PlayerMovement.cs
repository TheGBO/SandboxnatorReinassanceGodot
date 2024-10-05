using Godot;
using System;

public partial class PlayerMovement : AbstractPlayerComponent
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 8f;
	[Export] public CharacterBody3D cb;

	public override void _PhysicsProcess(double delta)
	{
		if(parent.IsMultiplayerAuthority()){
			cb.Velocity = Vector3.Zero;
			Vector3 velocity = cb.Velocity;

			// Add the gravity.
			if (!cb.IsOnFloor())
			{
				velocity += cb.GetGravity() * (float)delta;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("mv_jump") && cb.IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector3 forward = cb.GlobalTransform.Basis.Z;
			Vector3 right = cb.GlobalTransform.Basis.X;

			Vector2 inputDir = Input.GetVector("mv_left", "mv_right", "mv_forward", "mv_backward");
			Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();

			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(cb.Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(cb.Velocity.Z, 0, Speed);
			}

			cb.Velocity = velocity;
			cb.MoveAndSlide();
		}
	}
}
