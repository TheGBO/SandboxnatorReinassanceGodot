using Godot;
using NullGarel.Sandboxnator.Network;
using NullGarel.Util.ComponentSystem;
using System;
namespace NullGarel.Sandboxnator.Entity;

/// <summary>
/// Centralized component of Graphical User Interface to a player
/// TODO: Centralize other HUD elements to be held by this class instead of scattered through other scripts.
/// ? What hud elements exactly? i need to ellaborate this further xd
/// </summary>
[GodotClassName(nameof(PlayerHUD))]
public partial class PlayerHUD : AbstractComponent<Player>
{
    [ExportCategory("Main Controls")]
    [Export] public Control chatRoot;
    [Export] private Control _escMenu;
    [Export] private Control _hotBar;

    [ExportCategory("Grid vs snapper information")]
    [Export] private TextureRect _alignmentInformationIcon;
    [Export] private Texture2D _gridIcon;
    [Export] private Texture2D _snapperIcon;

    public bool IsChatOpen { get; set; }
    public bool IsHudBeingUsed { get; private set; }

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority())
            return;

        var playerInput = ComponentParent.playerInput;

        playerInput.OnUiEscape += () =>
        {
            if (IsChatOpen) return;
            //force mouse cursor to show up if it's not there
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Visible;

            _escMenu.Visible = !_escMenu.Visible;
        };

        playerInput.ChangeSnapMode += (bool isGrid) =>
        {
            _alignmentInformationIcon.Texture = isGrid ? _gridIcon : _snapperIcon;
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

        IsHudBeingUsed = IsChatOpen || _escMenu.Visible;
    }
}
