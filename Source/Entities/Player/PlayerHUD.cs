using Godot;
using NullCyan.Sandboxnator.Network;
using NullCyan.Util.ComponentSystem;
using System;
namespace NullCyan.Sandboxnator.Entity;

/// <summary>
/// Centralized component of Graphical User Interface to a player
/// TODO: Centralize other HUD elements to be held by this class instead of scattered through other scripts.
/// ? What hud elements exactly? i need to ellaborate this further xd
/// </summary>
[GodotClassName(nameof(PlayerHUD))]
public partial class PlayerHUD : AbstractComponent<Player>
{
    [Export] public Control chatRoot;
    [Export] private Control escMenu;
    [Export] private Control hotBar;

    public bool IsChatOpen { get; set; }
    public bool IsHudBeingUsed { get; private set; }

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority())
            return;

        ComponentParent.playerInput.OnUiEscape += () =>
        {
            if (IsChatOpen) return;
            //force mouse cursor to show up if it's not there
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Visible;

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
