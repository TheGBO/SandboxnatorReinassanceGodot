using Godot;
using Godot.Collections;
namespace NullGarel.Sandboxnator.Item;

/// <summary>
/// The data representation of a placing item in areas such as the inventory(as an ID) or the game registry in general.
/// </summary>
[GlobalClass]
public partial class PlaceableItemData : ItemData
{
    [ExportCategory("Placement")]
    [Export] public PackedScene BuildingScene { get; private set; }
    [Export] public float SnapRange { get; private set; } = 0.5f;
    [Export] public float NormalOffset { get; private set; } = 1;
    [Export] public Vector3 GridSize { get; private set; } = new(0.5f, 0.5f, 0.5f);
    [ExportCategory("Audio")]
    [Export] public AudioStream PlacementSound { get; private set; }
    [ExportCategory("Overrides")]
    [Export] public StandardMaterial3D MaterialOverride { get; private set; } = null;
}