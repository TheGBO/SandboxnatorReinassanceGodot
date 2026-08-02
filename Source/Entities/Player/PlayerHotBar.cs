using Godot;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using System;
namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerHotBar : Control
{
    [Export] private PlayerItemSync playerItemSync;
    [Export] private TextureRect activeItemIcon;

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
        playerItemSync.OnItemEquipped += UpdateActiveItemIcon;
    }


    private void UpdateActiveItemIcon(string itemID)
    {
        activeItemIcon.Texture = GameRegistries.Instance.ItemRegistry.Get(itemID).itemIcon;
    }
}