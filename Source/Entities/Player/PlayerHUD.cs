using Godot;
using NullCyan.Sandboxnator.Network;
using NullCyan.Util.ComponentSystem;
using System;
namespace NullCyan.Sandboxnator.Entity;

/// <summary>
/// Centralized component of Graphical User Interface to a player
/// TODO: Centralize other HUD elements to be held by this class instead of scattered through other scripts.
/// </summary>
[GodotClassName(nameof(PlayerHUD))]
public partial class PlayerHUD : AbstractComponent<Player>
{
    [Export] public Control chatRoot;
    [Export] private Control escMenu;
    public bool IsChatOpen { get; set; }
    public bool IsHudBeingUsed { get; private set; }

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority())
            return;

        ComponentParent.playerInput.OnToggleCursorCapture += () =>
        {
            if (IsChatOpen) return;

            escMenu.Visible = !escMenu.Visible;
        };
    }

    public void _on_leave_game_btn_pressed()
    {
        NetworkManager.Instance.QuitConnection();
        GetTree().ReloadCurrentScene();
    }

    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;

        IsHudBeingUsed = IsChatOpen || escMenu.Visible;
    }
}
