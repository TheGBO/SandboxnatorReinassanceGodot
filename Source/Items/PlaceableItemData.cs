using Godot;
using System;
namespace NullCyan.Sandboxnator.Item;

/// <summary>
/// The data representation of a placing item in areas such as the inventory(as an ID) or the game registry in general.
/// </summary>
[GlobalClass]
public partial class PlaceableItemData : ItemData
{
    [Export] public AudioStream placementSound;
}