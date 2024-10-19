using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Class that holds the world scene data
/// </summary>
public partial class World : Node3D
{
	public static World Instance { get; private set; }
	public List<Snapper> snappers = new List<Snapper>();

	[Export] public Node3D neworkedEntities;

	public override void _EnterTree()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector3 GetNearestSnapper(Vector3 referential, float maxRange)
	{
		foreach (Snapper snapper in snappers)
		{
			if ((referential.DistanceTo(snapper.GlobalPosition) <= maxRange) && !snapper.InsideBody)
			{
				return snapper.GlobalPosition;
			}
		}
		return referential;
	}
}
