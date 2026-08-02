using Godot;
using NullCyan.Sandboxnator.Entity;
using System;
namespace NullCyan.Sandboxnator.Item;

/// <summary>
/// Not to be confused with <see cref="ItemData"/> as this is a physical representation of a held item in-world.
/// </summary>
[GlobalClass]
public partial class BaseItem : Node3D
{
	/// <summary>
	/// The reference to the PlayerItemUse component
	/// </summary>
	public PlayerItemUse ItemUser { get; set; }
	[Export] public float RaycastRangeOverride { get; private set; } = 8;

	[Export] public bool animateHand;
	[Export] public float usageCooldown;

	/// <summary>
	/// Called on the server-side to validate and process item usage.
	/// </summary>
	/// <param name="args"></param>
	public virtual void UseItem(ItemUsageArgs args)
	{

	}

	/// <summary>
	/// Called when the server broadcasts a state update for this specific item.
	/// TODO: using a byte array is provisory btw. maybe some DTO class based on godot's dictionary?
	/// </summary>
	public virtual void ReceiveItemState(byte[] stateData)
	{
	}
}
