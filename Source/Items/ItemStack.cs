using Godot;
using Godot.Collections;

namespace NullGarel.Sandboxnator.Item;

[GlobalClass]
public partial class ItemStack : Resource
{
    [Export]
    public string ItemId { get; set; } = string.Empty;

    [Export]
    public int Amount { get; set; }

    [Export]
    public Dictionary StackData { get; set; } = [];

    public bool IsEmpty => string.IsNullOrEmpty(ItemId) || Amount <= 0;
}