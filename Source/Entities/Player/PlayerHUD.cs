using Godot;
using NullGarel.Sandboxnator.Network;
using NullGarel.Sandboxnator.UI;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.Log;
using System;
namespace NullGarel.Sandboxnator.Entity;

/// <summary>
/// Centralized component of Graphical User Interface to a player
/// </summary>
[GodotClassName(nameof(PlayerHUD))]
public partial class PlayerHUD : AbstractComponent<Player>, IUiSignalLoader
{
    [ExportCategory("Main Controls")]
    [Export] public Control chatRoot;
    [Export] private Control _hotBar;
    [ExportCategory("ESC Menu")]
    [Export] private Control _escMenu;
    [Export] private Button _leaveGameBtn;
    [Export] private Button _settingsBtn;

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

        ConnectUISignals();

        var playerInput = ComponentParent.playerInput;

        playerInput.OnUiEscape += () =>
        {
            if (IsChatOpen) return;
            //force mouse cursor to show up if it's not there
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Visible;

            _escMenu.Visible = !_escMenu.Visible;
        };

        playerInput.OnChangeSnapMode += isGrid =>
        {
            _alignmentInformationIcon.Texture = isGrid ? _gridIcon : _snapperIcon;
        };
    }

    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;

        IsHudBeingUsed = IsChatOpen || _escMenu.Visible;
    }

    public void ConnectUISignals()
    {
        //TODO: inject information onto the settings menu on whether return to world or to main menu
        _settingsBtn.Pressed += () => { NcLogger.Log("NOT IMPLEMENTED XD"); };
        _leaveGameBtn.Pressed += () =>
        {
            SandboxnatorMain.Instance.LeaveWorld();
        };
    }
}
