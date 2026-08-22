using Godot;
using NullGarel.Sandboxnator.Registry;
namespace NullGarel.Sandboxnator.Entity;

public partial class PlayerHotBar : Control
{
    [Export] private PlayerItemSync _playerItemSync;
    [Export] private TextureRect _activeItemIcon;

    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) return;
        _playerItemSync.OnItemEquipped += UpdateActiveItemIcon;
    }


    private void UpdateActiveItemIcon(string itemID)
    {
        _activeItemIcon.Texture = GameRegistries.Instance.ItemRegistry.Get(itemID).ItemIcon;
    }
}