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
        base._Ready();
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
        RpcId(1, nameof(C2S_RequestCycleItem), increment);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void C2S_RequestCycleItem(int increment)
    {
        if (!Multiplayer.IsServer() || inventory.Count == 0) return;

        _inventoryIndex += increment;
        string nextItemId = inventory[Mathf.Abs(_inventoryIndex % inventory.Count)];

        CurrentItemId = nextItemId;
        Rpc(nameof(S2C_ConfirmItemChange), nextItemId);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void S2C_ConfirmItemChange(string itemId)
    {
        CurrentItemId = itemId;
    }
}