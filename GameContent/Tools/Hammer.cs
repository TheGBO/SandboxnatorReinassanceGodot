using Godot;
using System;

public partial class Hammer : BaseTool
{
    public override void UseTool(Vector3 position, Vector3 normal)
    {
        GD.Print("STOP! Hammer time");
        //Node3D nut = (Node3D)nutScene.Instantiate();
		//nut.Name = Guid.NewGuid().GetHashCode().ToString();
		//CallDeferred("add_child", nut, true);
		//nut.Position = position;

    }
}
