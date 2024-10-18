using Godot;
using System;

public partial class PlayerInput : Node
{
	//movement
	public Action OnJumpButtonPressed;
	public Vector2 MovementVector { get; private set; }
	public bool IsSprinting { get; private set; }
	//user interface
	public Action OnToggleChatPressed;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;

		if (Input.IsActionJustPressed("mv_jump"))
		{
			OnJumpButtonPressed?.Invoke();
		}

		MovementVector = Input.GetVector("mv_left", "mv_right", "mv_forward", "mv_backward");
		IsSprinting = Input.IsActionPressed("mv_sprint");
	}
}
