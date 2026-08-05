using Godot;
using NullCyan.Util.ComponentSystem;
using NullCyan.Util.Log;
using System;

namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerItemSync : AbstractComponent<Player>
{
    [Export] private Godot.Collections.Array<string> inventory = [];

    private string _currentItemId = string.Empty;
    private int _inventoryIndex;

    public event Action<string> OnItemEquipped;

    [Export]
    public string CurrentItemId
    {
        get => _currentItemId;
        set
        {
            if (_currentItemId == value) return;
            _currentItemId = value;

            OnItemEquipped?.Invoke(_currentItemId);
        }
    }

    public override void _Ready()
    {
        // this component should be authority of the server.
        SetMultiplayerAuthority(1);
        if (!string.IsNullOrEmpty(_currentItemId))
        {
            NcLogger.Log($"NOTHING EVER HAPPENS? {_currentItemId}");
            OnItemEquipped?.Invoke(_currentItemId);
        }
        else if (Multiplayer.IsServer() && inventory.Count > 0)
        {
            CurrentItemId = inventory[0];
        }
    }

    public void RequestCycleItem(int increment)
    {
        RpcId(1, nameof(ServerBoundRequestCycleItem), increment);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void ServerBoundRequestCycleItem(int increment)
    {
        if (!Multiplayer.IsServer() || inventory.Count == 0) return;

        _inventoryIndex += increment;
        string nextItemId = inventory[Mathf.Abs(_inventoryIndex % inventory.Count)];

        CurrentItemId = nextItemId;
        Rpc(nameof(ClientBoundConfirmItemChange), nextItemId);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void ClientBoundConfirmItemChange(string itemId)
    {
        CurrentItemId = itemId;
    }
}