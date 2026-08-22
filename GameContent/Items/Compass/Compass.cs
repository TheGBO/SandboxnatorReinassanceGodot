using Godot;
using NullGarel.Sandboxnator.Item;

public partial class Compass : BaseItem
{
	[Export] public Node3D CompassPointer { get; private set; }
	[Export] public Label3D CoordinateLabel { get; private set; }

	public override void _Process(double delta)
	{
		Vector3 northTarget = CompassPointer.GlobalPosition + Vector3.Forward;
		CompassPointer.LookAt(northTarget, Vector3.Up);
		Vector3 playerPos = ItemUser.ComponentParent.GlobalPosition;
		string eastWest = $"{Mathf.Abs(playerPos.X):F0}{(playerPos.X > 0 ? "E" : "W")}";
		string southNorth = $"{Mathf.Abs(playerPos.Z):F0}{(playerPos.Z > 0 ? "S" : "N")}";

		CoordinateLabel.Text = $"{southNorth} ; {eastWest}";
	}
}
