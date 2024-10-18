using Godot;
using System;

public partial class PlayerMovement : AbstractPlayerComponent
{
	//movement
	[Export] public CharacterBody3D cb;
	private float currentSpeed = 5f;
	[Export] public float walkSpeed = 7.5f;
	[Export] public float sprintSpeed = 7.5f;
	[Export] public float jumpVelocity = 8f;

	//rigid body interaction
	[Export] public float mass = 5f;
	[Export] public float pushForceScalar = 2f;

	//visual effects
	[Export] public Camera3D camera;
	[Export] public AnimationPlayer neckAnimator;
	//TODO: fov able to be tweaked in settings.
	[Export] public float fov = 75;
	[Export] public float sprintFov = 100;
	[Export] public float sprintEffectTime = 0.75f;

	//TODO: Separate input from movement
	public override void _PhysicsProcess(double delta)
	{
		if (!parent.IsMultiplayerAuthority())
		{
			return;
		}

		camera.Fov = fov;

		Vector3 velocity = cb.Velocity;

		// Add the gravity.
		if (!cb.IsOnFloor())
		{
			velocity += cb.GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("mv_jump") && cb.IsOnFloor())
		{
			velocity.Y = jumpVelocity;
		}



		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector3 forward = cb.GlobalTransform.Basis.Z;
		Vector3 right = cb.GlobalTransform.Basis.X;

		Vector2 inputDir = Input.GetVector("mv_left", "mv_right", "mv_forward", "mv_backward");
		Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();
		bool isMoving = inputDir != Vector2.Zero;

		//check for sprint
		bool isSprinting = Input.IsActionPressed("mv_sprint");
		if (isSprinting)
		{
			Sprint(true);
		}
		else if (Input.IsActionJustReleased("mv_sprint"))
		{
			Sprint(false);
		}
		if (isMoving && !isSprinting)
		{
			neckAnimator.Play("walk");
		}

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * currentSpeed;
			velocity.Z = direction.Z * currentSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(cb.Velocity.X, 0, currentSpeed);
			velocity.Z = Mathf.MoveToward(cb.Velocity.Z, 0, currentSpeed);
		}

		cb.Velocity = velocity;
		cb.MoveAndSlide();

	}

	//not my code, adapted version from https://www.youtube.com/watch?v=Uh9PSOORMmA
	private void PushAwayRigidBodies()
	{
		for (int i = 0; i < cb.GetSlideCollisionCount(); i++)
		{
			KinematicCollision3D CollisionData = cb.GetSlideCollision(i);

			GodotObject UnkObj = CollisionData.GetCollider();

			if (UnkObj is RigidBody3D)
			{
				RigidBody3D Obj = UnkObj as RigidBody3D;
				float MassRatio = Mathf.Min(1.0f, mass / Obj.Mass);
				if (MassRatio < 0.25f) continue;
				Vector3 PushDir = -CollisionData.GetNormal();
				float VelocityDiffInPushDir = cb.Velocity.Dot(PushDir) - Obj.LinearVelocity.Dot(PushDir);
				VelocityDiffInPushDir = Mathf.Max(0.0f, VelocityDiffInPushDir);
				PushDir.Y = 0;
				float PushForce = MassRatio * pushForceScalar;
				Obj.ApplyImpulse(PushDir * VelocityDiffInPushDir * PushForce, CollisionData.GetPosition() - Obj.GlobalPosition);
			}
		}
	}



	private void Sprint(bool beginSprint)
	{
		neckAnimator.Play("sprint");
		Tween sprintTween = GetTree().CreateTween();
		if (beginSprint)
		{
			sprintTween.TweenProperty(camera, "fov", sprintFov, sprintEffectTime);
			sprintTween.TweenProperty(this, nameof(currentSpeed), sprintSpeed, sprintEffectTime);
		}
		else
		{
			sprintTween.TweenProperty(camera, "fov", fov, sprintEffectTime);
			sprintTween.TweenProperty(this, nameof(currentSpeed), walkSpeed, sprintEffectTime);
		}
	}
}
