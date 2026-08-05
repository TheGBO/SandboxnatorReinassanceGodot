using Godot;
using Godot.Collections;
using NullCyan.Util.ComponentSystem;
using NullCyan.Util.GodotHelpers;
using System;

namespace NullCyan.Sandboxnator.Building;

public partial class Paintable : AbstractComponent<Placeable>
{
    [Export] private Array<MeshInstance3D> targetMeshes;
    private Color currentColor = new(1, 1, 1); //default white

    public override void _Ready()
    {
        // this component should be authority of the server.
        SetMultiplayerAuthority(1);
    }

    public void TriggerPaint(Color color)
    {
        if (Multiplayer.IsServer())
            Rpc(nameof(ClientBoundReceivePaint), color);
        else
            RpcId(1, nameof(ServerBoundRequestPaint), color);

        ApplyColor(color);
    }

    private void ApplyColor(Color color)
    {
        currentColor = color;
        foreach (var mesh in targetMeshes)
            ColorAndMeshUtils.ChangeMeshColor(mesh, color);
    }

    #region SYNC
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void ClientBoundReceivePaint(Color color)
    {
        ApplyColor(color);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ServerBoundRequestPaint(Color color)
    {
        Rpc(nameof(ClientBoundReceivePaint), color);
    }
    //if this is instanced, even on a client, the client will ask the server to trigger a sync, manual but works i guess xd;
    // TODO: I should probably make a proper abstract sync (OR SYNC COMPONENT :3) before this gets out of hand
    public override void _EnterTree()
    {
        if (!Multiplayer.IsServer())
            RpcId(1, nameof(RequestSync));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SyncColor(Color color)
    {
        ApplyColor(color);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void RequestSync()
    {
        RpcId(Multiplayer.GetRemoteSenderId(), nameof(SyncColor), currentColor);
    }
    #endregion
}
