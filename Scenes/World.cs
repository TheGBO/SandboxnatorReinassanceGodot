using Godot;
using System;

/// <summary>
/// Class that holds the world scene data
/// </summary>
public partial class World : Node3D
{
	public static World Instance { get; private set; }
	[Export] public Node3D neworkedEntities;

	public override void _EnterTree()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
