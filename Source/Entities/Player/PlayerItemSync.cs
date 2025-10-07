using Godot;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util.ComponentSystem;

namespace NullCyan.Sandboxnator.Entity;
//TODO: Make a basic abstract synchronize class to handle stuff
public partial class PlayerItemSync : AbstractComponent<Player>
{
    public override void _Ready()
    {
        if (Multiplayer.IsServer() && World.HasInstance())
        {
            World.Instance.OnPlayerJoin += _ =>
            {
                ServerForceSync(ComponentParent.playerItemUse.CurrentItemID);
            };
        }
    }

    /// <summary>
    /// Called only by the server to push item state to all clients.
    /// </summary>
    public void ServerForceSync(string itemId)
    {
        //broadcast the item id and trigger update
        Rpc(nameof(S2C_SetItem), itemId);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void S2C_SetItem(string itemId)
    {
        //GD.Print(itemId);
        var itemUse = ComponentParent.playerItemUse;
        if (itemUse != null)
            itemUse.SetItemFromNetwork(itemId);
    }
}
