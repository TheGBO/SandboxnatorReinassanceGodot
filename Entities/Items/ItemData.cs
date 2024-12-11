using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    //Contains the model and the logic
    [ExportGroup("Basic properties")]
    [Export] public PackedScene itemScene;
    [Export] public string itemID;
    [Export] public string itemName;
    [ExportGroup("Practical use")]
    [Export] public float usageCooldown;
    [Export] public float raycastRange;
}
