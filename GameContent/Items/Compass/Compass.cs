using Godot;
using NullCyan.Sandboxnator.Item;
using System;

public partial class Compass : BaseItem
{
	[Export] public Node3D CompassPointer { get; set; }

	public override void _Process(double delta)
	{
		Vector3 northTarget = CompassPointer.GlobalPosition + Vector3.Forward;
		CompassPointer.LookAt(northTarget, Vector3.Up);
	}
}
