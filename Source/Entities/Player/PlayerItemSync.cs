using System;
using Godot;
using Godot.Collections;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.Log;

namespace NullGarel.Sandboxnator.Entity;

/// <summary>
/// Component responsible for item synchronization and inventory logic
/// </summary>
public partial class PlayerItemSync : AbstractComponent<Player>
{
    [Export] private Array<string> inventory = [];
    private int _inventoryIndex;
    public event Action<string> OnItemEquipped;

    private string _currentItemId = string.Empty;
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

    private Dictionary _activeItemState;
    public Dictionary ActiveItemState
    {
        get => _activeItemState;
        set
        {
            if (_activeItemState == value) return;
            _activeItemState = value;
        }
    }

    public override void _Ready()
    {
        // this component should be authority of the server.
        SetMultiplayerAuthority(1);

        if (!string.IsNullOrEmpty(_currentItemId))
        {
            OnItemEquipped?.Invoke(_currentItemId);
        }
        else if (Multiplayer.IsServer() && inventory.Count > 0)
        {
            CurrentItemId = inventory[0];
        }

        if (Multiplayer.IsServer())
        {
            // address the late joiner issue I suppose
            Multiplayer.PeerConnected += OnPeerConnected;
        }
    }

    private void OnPeerConnected(long id)
    {
        BroadcastItemState(ActiveItemState);
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

    /// <summary>
    /// broadcast state changes to clients.
    /// </summary>
    public void BroadcastItemState(Dictionary stateData)
    {
        if (Multiplayer.IsServer())
        {
            ActiveItemState = stateData;
            Rpc(nameof(ClientBoundSyncItemState), stateData);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void ClientBoundSyncItemState(Dictionary stateData)
    {
        ActiveItemState = stateData;
        ComponentParent.playerItemUse.Item?.ReceiveItemState(ActiveItemState);
    }
}