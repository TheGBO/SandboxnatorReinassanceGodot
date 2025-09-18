using Godot;
using NullCyan.Sandboxnator.UI;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util;
using System;
namespace NullCyan.Sandboxnator;

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
