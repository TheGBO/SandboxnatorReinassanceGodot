using Godot;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using System;
namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerHotBar : Control
{
    [Export] private PlayerItemUse playerItemUse;
    [Export] private TextureRect activeItemIcon;

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
        playerItemUse.OnItemChanged += UpdateActiveItemIcon;
    }


    private void UpdateActiveItemIcon(string itemID)
    {
        activeItemIcon.Texture = GameRegistries.Instance.ItemRegistry.Get(itemID).itemIcon;
    }
}