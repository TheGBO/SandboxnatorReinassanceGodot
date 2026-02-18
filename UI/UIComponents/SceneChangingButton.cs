using Godot;
using NullCyan.Util.Log;
using System;
namespace NullCyan.Sandboxnator.UI;

/// <summary>
/// A button that changes the current scene to another, this is intended for use in menus, not the in-world.
/// </summary>
public partial class SceneChangingButton : Button
{
    [Export] private PackedScene targetScene;

    public override void _Ready()
    {
        Pressed += () =>
        {
            if (targetScene == null)
            {
                NcLogger.Error("Error: null target scene.");
                return;
            }
            GetTree().ChangeSceneToPacked(targetScene);
        };
    }
}
