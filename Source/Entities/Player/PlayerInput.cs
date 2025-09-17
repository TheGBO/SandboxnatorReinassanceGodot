using Godot;
using System;
using NullCyan.Util.ComponentSystem;
using NullCyan.Sandboxnator.Registry;
namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerInput : AbstractComponent<Player>
{
    //movement
    public Vector2 MovementVector { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }
    public Action OnStopSprint;
    //user interface

    public Action OnShowChat;
    public Action OnUiEscape;
    //Camera
    public Action OnToggleCursorCapture;
    public Vector2 LookVector { get; private set; }
    public Action OnMouseMovement;
    //Building
    public Action RotateCW;
    public Action RotateCCW;
    //usage
    public Action UsePrimary;
    public Action UseIncrement;
    public Action UseDecrement;

    private const float JOYPAD_SENSITIVITY_DENOMINATOR = 100.0f;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;
        if (!ComponentParent.playerHud.IsHudBeingUsed)
        {
            HandleMovementInput();
            HandleBuildingInput();
            HandleUsageInput();
            HandleJoypadRstickInput();
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

        if (Input.IsActionJustPressed("toggle_capture"))
        {
            OnToggleCursorCapture?.Invoke();
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

    private void HandleJoypadRstickInput()
    {
        Vector2 joypadLookVector = new Vector2(Input.GetAxis("look_left", "look_right"), Input.GetAxis("look_up", "look_down"));
        if (joypadLookVector.Length() > 0.1f)
        {
            // There is a proportion matter when it comes to this.
            // The default (raw) mouse sensitivity is 100, the default joypad sensitivity is 5.
            // in order to turn 100 into 5 in a proportional way, (5 * sens)/100
            // TOTEST: this needs further testing
            LookVector = joypadLookVector * (5 * GameRegistries.Instance.SettingsData.Sensitivity) / JOYPAD_SENSITIVITY_DENOMINATOR;
            OnMouseMovement?.Invoke();
        }
    }

    public override void _Input(InputEvent _event)
    {
        if (ComponentParent.IsMultiplayerAuthority())
        {
            if (ComponentParent == null)
            {
                GD.Print("NULL PARENT WARNING");
            }
            if (_event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                Vector2 mouseLookVector = new Vector2(mouseMotion.Relative.X, mouseMotion.Relative.Y);
                LookVector = mouseLookVector;
                //GD.Print($"mouse motion {mouseLookVector}");
                OnMouseMovement?.Invoke();
            }


        }
    }
}
