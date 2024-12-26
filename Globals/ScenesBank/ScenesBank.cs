using Godot;
using System;

public partial class ScenesBank : Node
{
    public static ScenesBank Instance { get; private set; }

    [Export] public PackedScene worldScene;
    [Export] public PackedScene profileScene;
    [Export] public PackedScene mainMenuScene;

    public override void _Ready()
    {
        UiSoundManager.Instance.TryInstallSounds();
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
