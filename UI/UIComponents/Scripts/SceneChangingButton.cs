using Godot;
using NullGarel.Util.Log;
namespace NullGarel.Sandboxnator.UI;

/// <summary>
/// A button that changes the current scene to another, this is intended for use in menus, not the in-world.
/// </summary>
public partial class SceneChangingButton : Button
{
    [Export(PropertyHint.File, "*.tscn")]
    public string targetScene;

    public override void _Ready()
    {
        Pressed += () =>
        {
            if (targetScene == null || string.IsNullOrEmpty(targetScene))
            {
                NcLogger.Error("Error: null target scene.");
                return;
            }
            GetTree().ChangeSceneToFile(targetScene);
        };
    }
}
