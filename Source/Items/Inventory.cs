using Godot;
using Godot.Collections;
namespace NullGarel.Sandboxnator.Item;

[GlobalClass]
public partial class Inventory : Node
{
    public const int SlotCount = 32;

    [Export]
    private ItemStack[] _slots = new ItemStack[SlotCount];

    public int Count => SlotCount;

    public ItemStack GetSlot(int index)
        => _slots[index];

    public void SetSlot(int index, ItemStack stack)
        => _slots[index] = stack;
}