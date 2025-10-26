using Godot;
using NullCyan.Sandboxnator.Building;
using System;
namespace NullCyan.Sandboxnator.Item;

//the best item for looking up when if forget stuff
public partial class Hammer : BaseItem
{
	public override void UseItem(ItemUsageArgs args)
	{
		var hitObject = ItemUser.rayCast.GetCollider();
		if (hitObject is Placeable)
		{
			Placeable hitPlaceable = (Placeable)hitObject;
			hitPlaceable.Destroy();
		}
	}
}
