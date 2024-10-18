using Godot;
using System;

public partial class Hammer : BaseTool
{
    public override void UseTool(ToolUsageArgs args)
    {
        GD.Print($"STOP! Hammer time, Hammer usage received from player {args.PlayerId}");
        //Node3D nut = (Node3D)nutScene.Instantiate();
        //nut.Name = Guid.NewGuid().GetHashCode().ToString();
        //CallDeferred("add_child", nut, true);
        //nut.Position = position;

    }
}
