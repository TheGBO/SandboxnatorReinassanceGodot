using Godot;
using NullCyan.Util.ComponentSystem;

namespace NullCyan.Sandboxnator.Entity;

public partial class PlayerItemSync : AbstractComponent<Player>
{
    /// <summary>
    /// Called only by the server to push item state to all clients.
    /// </summary>
    public void ServerForceSync(string itemId)
    {
        if (ComponentParent.IsMultiplayerAuthority())
            Rpc(nameof(S2C_SetItem), itemId);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void S2C_SetItem(string itemId)
    {
        var itemUse = ComponentParent.playerItemUse;
        if (itemUse != null)
            itemUse.SetItemFromNetwork(itemId);
    }
}
