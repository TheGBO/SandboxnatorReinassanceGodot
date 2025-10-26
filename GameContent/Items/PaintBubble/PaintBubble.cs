using Godot;
using NullCyan.Sandboxnator.Building;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.GodotHelpers;
using NullCyan.Util.Log;
using System;
using System.Linq;

namespace NullCyan.Sandboxnator.Item;

public partial class PaintBubble : BaseItem
{
    [Export] private MeshInstance3D bubble;
    [Export] private int colorIndex = 0;

    private Color[] _colors = ColorAndMeshUtils.PixelsOfImage(GameRegistries.Instance.BuildingPallete.GetImage()).ToArray();

    public override void _EnterTree()
    {
        UpdateVisual();
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
        NcLogger.Warn("not implemented yet god damn it");
        colorIndex = (colorIndex + 1) % _colors.Length;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        ColorAndMeshUtils.ChangeMeshColor(bubble, _colors[colorIndex]);
    }
}
