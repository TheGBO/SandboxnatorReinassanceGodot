using Godot;
using NullCyan.Sandboxnator.Building;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util.Log;
using System;
using System.Linq;

namespace NullCyan.Sandboxnator.Item;

//TODO: There's something really important to be done: store data in this item persistently.
// For instance, if you change the color and scroll to another item and scroll back to this
// one, the color is reset and completely forgotten.
// I'm thinking of either a dictionary or a msgpack object for this purpose.
// Minecraft does this for things like durability, custom item names, custom properties, etc..
// And I think this is what NBT does on minecraft.
// I don't even know if this is inspiration or plagiarizing at this point xd
public partial class PaintBubble : BaseItem
{
    [Export] private MeshInstance3D bubble;
    [Export] private int colorIndex = 0;

    private Color[] _colors = ColorAndMeshUtils.PixelsOfImage(GameRegistries.Instance.BuildingPallete.GetImage()).ToArray();

    public override void _EnterTree()
    {
        UpdateVisualLocal();
    }

    public override void UseItem(ItemUsageArgs args)
    {
        if (args.IsPrimaryUse)
        {
            var hitObject = ItemUser.rayCast.GetCollider();
            if (hitObject is not Placeable hitPlaceable)
                return;

            var paintable = hitPlaceable.componentHolder
                .GetChildren()
                .OfType<Paintable>()
                .FirstOrDefault();

            if (paintable == null)
            {
                NcLogger.Log($"Missing paintable component in {hitPlaceable.Name}");
                return;
            }

            paintable.TriggerPaint(_colors[colorIndex]);
        }
        else
        {
            CycleColor();
        }
    }

    private void CycleColor()
    {
        colorIndex = (colorIndex + 1) % _colors.Length;
        // Delegate network broadcast to the permanent component
        ItemUser.BroadcastItemState([(byte)colorIndex]);
    }

    // this used to be a whole RPC thing but now it's an overriden method from BaseItem
    public override void ReceiveItemState(byte[] stateData)
    {
        if (stateData.Length > 0)
        {
            colorIndex = stateData[0];
            UpdateVisualLocal();
        }
    }

    private void UpdateVisualLocal()
    {
        ColorAndMeshUtils.ChangeMeshColor(bubble, _colors[colorIndex]);
    }
}