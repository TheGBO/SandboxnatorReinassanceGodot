using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Entity;
namespace NullGarel.Sandboxnator.Item;

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
	[Export] public ItemData itemData;

	/// <summary>
	/// Called on the server-side to validate and process item usage.
	/// </summary>
	/// <param name="args"></param>
	public virtual void UseItem(ItemUsageArgs args)
	{

	}

	/// <summary>
	/// Called when the server broadcasts a state update for this specific item.
	/// I should ellaborate on what an item state data is.
	/// </summary>
	public virtual void ReceiveItemState(Dictionary stateData)
	{
	}


	public virtual Dictionary GetItemState()
	{
		return new Dictionary { { "key", "value" } };
	}
}
