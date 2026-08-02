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
        //Since UseItem is server side, a rpc is required to also update the colour for the client.
        Rpc(nameof(S2C_UpdateVisual), colorIndex);
    }


    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void S2C_UpdateVisual(int index)
    {
        //set the colorIndex on the client to the 'index" received via network.
        colorIndex = index;
        UpdateVisualLocal();
    }

    /// <summary>
    /// Called to locally update the visual based on the current colorIndex, not necessarily
    /// network dependent as seen in the _EnterTree usage.
    /// </summary>
    private void UpdateVisualLocal()
    {
        ColorAndMeshUtils.ChangeMeshColor(bubble, _colors[colorIndex]);
    }
}
