using Godot;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util.IO;

namespace NullCyan.Sandboxnator.Network;
//WARNING: THIS CLASS WAS AI GENERATED FOR PROTOTYPE PURPOSES, DO NOT IMPLEMENT IT FULLY IN "PRODUCTION" YET.
/// <summary>
/// Generic network sync component for any type T.
/// Supports server-authoritative sync with optional client requests.
/// Tracks changes and only sends updates when the value actually changes.
/// </summary>
public abstract partial class AbstractSync<T> : Node
{
    [Export] public bool SyncOnJoin = true; // Sync current state to new players joining

    private T _currentValue;
    private bool _dirty = false;

    /// <summary>Current authoritative value</summary>
    public T Value
    {
        get => _currentValue;
        set
        {
            if (!Equals(_currentValue, value))
            {
                _currentValue = value;
                _dirty = true;

                if (IsMultiplayerAuthority())
                    ServerForceSync(_currentValue);
                else
                    RequestServerSync(_currentValue);

                OnValueChanged(_currentValue);
            }
        }
    }

    public override void _Ready()
    {
        if (Multiplayer.IsServer() && SyncOnJoin)
        {
            // Push current value to new players
            World.Instance.OnPlayerJoin += _ => ServerForceSync(_currentValue);
        }
    }

    /// <summary>Called whenever Value changes locally</summary>
    protected virtual void OnValueChanged(T newValue) { }

    #region Networking

    /// <summary>Server pushes authoritative state to all clients</summary>
    protected void ServerForceSync(T data)
    {
        byte[] packed = MPacker.Pack(data);
        Rpc(nameof(S2C_Sync), packed);
        _dirty = false;
    }

    /// <summary>Client requests server to update state</summary>
    protected void RequestServerSync(T data)
    {
        byte[] packed = MPacker.Pack(data);
        RpcId(1, nameof(C2S_Sync), packed);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void C2S_Sync(byte[] packedData)
    {
        // Server receives request from client
        T data = MPacker.Unpack<T>(packedData);
        OnServerReceive(data);
        // broadcast to all clients
        Rpc(nameof(S2C_Sync), packedData);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void S2C_Sync(byte[] packedData)
    {
        // Client receives authoritative state
        T data = MPacker.Unpack<T>(packedData);
        _currentValue = data;
        OnClientReceive(data);
    }

    /// <summary>Override to handle server-side processing</summary>
    protected abstract void OnServerReceive(T data);

    /// <summary>Override to handle client-side updates</summary>
    protected abstract void OnClientReceive(T data);

    #endregion
}
