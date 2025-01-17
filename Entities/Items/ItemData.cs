using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    //Contains the model and the logic
    [ExportGroup("Basic properties")]
    //The item scene that contains its model and functionality.
    [Export] public PackedScene itemScene;
    //The language-agnostic item id, use_snake_case_please
    [Export] public string itemID;
    //The item name, can be changed according to Locales.
    [Export] public string itemName;
    [ExportGroup("Practical use")]
    [Export] public float usageCooldown;
    [Export] public float raycastRange;
}
