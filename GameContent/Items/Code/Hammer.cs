using Godot;
using System;

public partial class Hammer : BaseItem
{
    public override void UseItem(ItemUsageArgs args)
    {
        GD.Print($"STOP! Hammer time, Hammer usage received from player {args.PlayerId}");
        var hitObject = Ptu.rayCast.GetCollider();
        if (hitObject is Building)
        {
            Building hitBuilding = (Building)hitObject;
            //TODO: destroy animation
            hitBuilding.QueueFree();
        }
    }
}
