using Godot;
using System;

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
