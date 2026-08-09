using Godot;
using NullGarel.Sandboxnator.UI;
using NullGarel.Util.GodotHelpers;
using NullGarel.Util;
using System;
namespace NullGarel.Sandboxnator;

public partial class ScenesBank : Singleton<ScenesBank>
{

    [Export] public PackedScene worldScene;
    [Export] public PackedScene profileScene;
    [Export] public PackedScene mainMenuScene;

    public override void _Ready()
    {
        UiSoundManager.Instance.TryInstallSounds();
    }
}
