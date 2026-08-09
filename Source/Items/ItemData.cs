using Godot;
using System;
namespace NullGarel.Sandboxnator.Item;

/// <summary>
/// The data representation of an item in areas such as the inventory(as an ID) or the game registry in general.
/// </summary>
[GlobalClass]
public partial class ItemData : Resource
{
    //Contains the model and the logic
    [ExportGroup("Basic properties")]
    //The item scene that contains its model and functionality.
    [Export] public PackedScene itemScene;
    //The language-agnostic item id, PascalCaseIsRecommended
    [Export] public string itemID;
    [ExportGroup("Visual information")]
    [Export] public Texture2D itemIcon;
    //The item name, in the future I'll make it so it can be changed according to Locales.
    [Export] public string itemName;
}
