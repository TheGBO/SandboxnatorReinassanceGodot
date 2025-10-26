using Godot;
using NullCyan.Sandboxnator.Entity;
using System;
namespace NullCyan.Sandboxnator.Item;

[GlobalClass]
public partial class BaseItem : Node3D
{
	/// <summary>
	/// abbreviation for Player Tool Use
	/// </summary>
	public PlayerItemUse ItemUser { get; set; }
	[Export] public float RaycastRangeOverride { get; private set; } = 8;

	[Export] public bool animateHand;
	[Export] public float usageCooldown;

	public virtual void UseItem(ItemUsageArgs args)
	{

	}
}
