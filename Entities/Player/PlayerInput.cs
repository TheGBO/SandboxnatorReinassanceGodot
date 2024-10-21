using Godot;
using System;

public partial class PlayerInput : AbstractPlayerComponent
{
    //movement
    public Vector2 MovementVector { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }
    public Action OnStopSprint;
    //user interface
    public bool IsChatOpen { get; set; }
    public Action OnShowChat;
    public Action OnUiEscape;
    //Camera
    public Action OnToggleCursorCapture;
    //Building
    public Action RotateCW;
    public Action RotateCCW;
    //usage
    public Action UsePrimary;
    public Action UseIncrement;
    public Action UseDecrement;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;
        if (!IsChatOpen)
        {
            HandleMovementInput();
            HandleBuildingInput();
            HandleUsageInput();
        }
        HandleUserInterfaceInput();
    }

    private void HandleUserInterfaceInput()
    {
        if (Input.IsActionJustPressed("ui_show_chat"))
        {
            OnShowChat?.Invoke();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (Input.IsActionJustPressed("ui_escape"))
        {
            OnUiEscape?.Invoke();
        }
    }

    private void HandleMovementInput()
    {

        if (Input.IsActionJustPressed("mv_jump"))
        {
            IsJumping = true;
        }

        if (Input.IsActionJustReleased("mv_jump"))
        {
            IsJumping = false;
        }

        if (Input.IsActionJustReleased("mv_sprint"))
        {
            OnStopSprint?.Invoke();
        }

        if (Input.IsActionJustPressed("toggle_capture"))
        {
            OnToggleCursorCapture?.Invoke();
        }

        MovementVector = Input.GetVector("mv_left", "mv_right", "mv_forward", "mv_backward");
        IsSprinting = Input.IsActionPressed("mv_sprint");
    }

    private void HandleBuildingInput()
    {
        if (Input.IsActionJustPressed("build_rotate_cw"))
        {
            RotateCW?.Invoke();
        }

        if (Input.IsActionJustPressed("build_rotate_ccw"))
        {
            RotateCCW?.Invoke();
        }
    }

    private void HandleUsageInput()
    {
        if (Input.IsActionJustPressed("use_primary"))
        {
            UsePrimary?.Invoke();
        }

        if (Input.IsActionJustPressed("use_increment"))
        {
            UseIncrement?.Invoke();
        }

        if (Input.IsActionJustPressed("use_decrement"))
        {
            UseDecrement?.Invoke();
        }
    }
}
