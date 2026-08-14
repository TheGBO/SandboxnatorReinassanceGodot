using Godot;
using NullGarel.Sandboxnator.UI;
using NullGarel.Util.GodotHelpers;
using NullGarel.Util;
using System;
namespace NullGarel.Sandboxnator;

[Obsolete("do NOT use this shit anymore as of sandboxnator 0.5.0")]
public partial class ScenesBank : Singleton<ScenesBank>
{

    [Export] public PackedScene worldScene;
    [Export] public PackedScene profileScene;
    [Export] public PackedScene mainMenuScene;

    public override void _Ready()
    {
    }
}
