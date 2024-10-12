using Godot;
using System;
using System.Collections.Generic;

public partial class BuildingManager : Node3D
{
	public static BuildingManager Instance { get; private set; }

	public List<BuildingState> buildingStates = new List<BuildingState>();

    public override void _EnterTree()
    {
		Instance = this;
    }

	public void AddBuilding(BuildingState state)
	{
		//Node3D nut = (Node3D)nutScene.Instantiate();
		//nut.Name = Guid.NewGuid().GetHashCode().ToString();
		//CallDeferred("add_child", nut, true);
		//nut.Position = position;

	}
}
