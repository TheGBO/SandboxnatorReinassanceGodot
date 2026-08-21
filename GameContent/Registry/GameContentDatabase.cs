using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Entity.PlayerCosmetics;
using NullGarel.Sandboxnator.Item;

namespace NullGarel.Sandboxnator.Registry;

[GlobalClass]
public partial class GameContentDatabase : Resource
{
    [Export] public Array<ItemData> Items { get; private set; }
    [Export] public Array<PlayerFaceData> PlayerFaces { get; private set; }
    [Export] public Texture2D BuildingPallete { get; private set; }
}