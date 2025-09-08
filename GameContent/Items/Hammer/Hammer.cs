using Godot;
using NullCyan.Sandboxnator.Building;
using System;
namespace NullCyan.Sandboxnator.Item;

public partial class Hammer : BaseItem
{
	public override void UseItem(ItemUsageArgs args)
	{
		//GD.Print($"STOP! Hammer time, Hammer usage received from player {args.PlayerId}");
		var hitObject = ItemUser.rayCast.GetCollider();
		if (hitObject is Placeable)
		{
			Placeable hitPlaceable = (Placeable)hitObject;
			hitPlaceable.Destroy();
		}
	}
}
