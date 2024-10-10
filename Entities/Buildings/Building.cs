using Godot;
using System;


//todo: make proper network manager
public partial class Building : Node3D
{
    public long buildingID;
    [Export(PropertyHint.File, "*.tscn")] public string scenePath;

    public void SetBuildingID()
    {
        buildingID = Guid.NewGuid().GetHashCode();
    }
}