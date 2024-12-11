using Godot;
using System;

[Tool]
public partial class BaseItem : Node3D
{
	/// <summary>
	/// abbreviation for Player Tool Use
	/// </summary>
	public PlayerItemUse Ptu { get; set; }
	[Export] public bool animateHand;

	public virtual void UseItem(ItemUsageArgs args)
	{

	}
}
