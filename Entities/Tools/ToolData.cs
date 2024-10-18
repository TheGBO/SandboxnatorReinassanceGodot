using Godot;
using System;

[GlobalClass]
public partial class ToolData : Resource
{
    //Contains the model and the logic
    [ExportGroup("Basic properties")]
    [Export] public PackedScene toolScene;
    [Export] public string toolID;
    [Export] public string toolName;
    [ExportGroup("Practical use")]
    [Export] public float usageCooldown;
    [Export] public float raycastRange;
}
