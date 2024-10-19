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
        if (!Multiplayer.IsServer())
        {
            RpcId(1, nameof(ServerHandleMessage), message.ToDictionary());
        }
        else
        {
            ServerHandleMessage(message.ToDictionary());
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ServerHandleMessage(Dictionary msg)
    {
        ChatMessage message = ChatMessage.FromDictionary(msg);
        GD.Print($"message received: {message.Content} from {message.PlayerId}");
        ClientReceiveMessage(msg);
        Rpc(nameof(ClientReceiveMessage), message.ToDictionary());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ClientReceiveMessage(Dictionary msg)
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
        ClientReceiveMessage(message.ToDictionary());
        Rpc(nameof(ClientReceiveMessage), message.ToDictionary());
    }
}
