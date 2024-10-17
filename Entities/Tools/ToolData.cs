using Godot;
using System;

[GlobalClass]
public partial class ToolData : Resource
{
    //Contains the model and the logic
    [Export] public PackedScene tool;
    [Export] public string toolName;
    [Export] public float usageCooldown;

}
