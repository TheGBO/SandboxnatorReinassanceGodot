using Godot;
using NullCyan.Util.ComponentSystem;

namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerItemSync : AbstractComponent<Player>
{
    private string _currentItemId = string.Empty;

    /// <summary>
    /// Now this thing is synced automatically across peers via MultiplayerSynchronizer.
    /// </summary>
    [Export]
    public string CurrentItemId
    {
        get => _currentItemId;
        set
        {
            _currentItemId = value;
            OnItemIdChanged(_currentItemId);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        // If a value was already synchronized before _Ready ran, apply it immediately.
        if (!string.IsNullOrEmpty(_currentItemId))
        {
            OnItemIdChanged(_currentItemId);
        }
    }

    public void SetEquippedItem(string itemId)
    {
        CurrentItemId = itemId;
    }

    private void OnItemIdChanged(string itemId)
    {
        var itemUse = ComponentParent?.playerItemUse;
        if (itemUse != null)
        {
            itemUse.SetItemFromNetwork(itemId);
        }
    }


}