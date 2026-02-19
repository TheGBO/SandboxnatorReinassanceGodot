using Godot;
using NullCyan.Util.ComponentSystem;
using System;
namespace NullCyan.Sandboxnator.Building;

public partial class Placeable : RigidBody3D
{
    //TODO: destroy animation
    [Export] public ComponentHolder componentHolder;
    [Export] public AudioStreamPlayer3D placedSound;

    public void Destroy()
    {
        QueueFree();
    }

    /// <summary>
    /// Called on the server side when the building is placed!!!
    /// </summary>
    public void S_OnPlaced()
    {
        Rpc(nameof(S2C_OnPlaced));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void S2C_OnPlaced()
    {
        placedSound?.Play();
    }
}
