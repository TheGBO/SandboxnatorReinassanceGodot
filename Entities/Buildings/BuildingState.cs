using Godot;
using System;


//Representation of a building that is in the world, cannot be sent over network
public partial class BuildingState : RigidBody3D
{
    public long buildingID;
    [Export(PropertyHint.ResourceType, "*.tscn")] public string resourcePath;

    public void SetBuildingID()
    {
        buildingID = Guid.NewGuid().GetHashCode();
    }
}

