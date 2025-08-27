using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class ChatManager : Node3D
{
    public static ChatManager Instance { get; private set; }
    public List<ChatMessage> messages;
    public Action<ChatMessage> OnMessageReceived;

    public override void _EnterTree()
    {
        Instance = this;
    }

    //called on client
    public void SendMessage(string msg)
    {
        ChatMessage message = new ChatMessage(msg, NetworkManager.Instance.peer.GetUniqueId());
        RpcId(1, nameof(C2S_HandleMessage), message.ToDictionary());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void C2S_HandleMessage(Dictionary msg)
    {
        ChatMessage message = ChatMessage.FromDictionary(msg);
        GD.Print($"message received: {message.Content} from {message.PlayerId}");
        if (message.Content.StartsWith("!PlayerList"))
        {
            GD.Print(World.Instance.GetPlayers());
        }
        else
        {
            S2C_ReceiveMessage(msg);
            Rpc(nameof(S2C_ReceiveMessage), message.ToDictionary());
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void S2C_ReceiveMessage(Dictionary msg)
    {
        ChatMessage message = ChatMessage.FromDictionary(msg);
        GD.Print(message.Content);
        OnMessageReceived?.Invoke(message);
    }

    /// <summary>
    /// SERVER SIDE: send message from server to client without attached player
    /// </summary>
    public void BroadcastPlayerlessMessage(string msg)
    {
        if (!Multiplayer.IsServer())
        {
            throw new InvalidOperationException("This operation can not be called on the client.");
        }
        //-1 playerless
        ChatMessage message = new ChatMessage(msg, -1);
        S2C_ReceiveMessage(message.ToDictionary());
        Rpc(nameof(S2C_ReceiveMessage), message.ToDictionary());
    }

    /// <summary>
    /// SERVER SIDE: Sends a private message to a single player without attached player
    /// </summary>
    /// <param name="msg">content</param>
    /// <param name="playerId">player</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SendPlayerlessMessage(string msg, int playerId)
    {
        if (!Multiplayer.IsServer())
        {
            throw new InvalidOperationException("This operation can not be called on the client.");
        }
        //-1 playerless
        ChatMessage message = new ChatMessage(msg, -1);
        S2C_ReceiveMessage(message.ToDictionary());
        RpcId(playerId, nameof(S2C_ReceiveMessage), message.ToDictionary());
    }
}
