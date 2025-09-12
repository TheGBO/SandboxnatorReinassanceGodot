using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using NullCyan.Util;
using NullCyan.Sandboxnator.Network;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Commands;
namespace NullCyan.Sandboxnator.Chat;


/// <summary>
/// Singleton responsible for sending, receiving, parsing and handling chat messages and commands.
/// </summary>
public partial class ChatManager : Singleton<ChatManager>
{
    public List<ChatMessage> messages;
    public Action<ChatMessage, PlayerProfileData> OnMessageReceived;

    //called on client
    public void RequestSendMessageToServer(string msg)
    {
        ChatMessage message = new ChatMessage(msg, NetworkManager.Instance.peer.GetUniqueId());
        RpcId(1, nameof(C2S_HandleMessage), MPacker.Pack(message));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void C2S_HandleMessage(byte[] messageBytes)
    {
        ChatMessage message = MPacker.Unpack<ChatMessage>(messageBytes);
        GD.Print($"message received:{message.Content.Replace("\n", "|CRLF|")} from {message.PlayerId}");
        Player sender = World.Instance.GetPlayerById(message.PlayerId);

        // Pass message to command system first
        if (!CommandRegistryManager.ExecuteCommand(sender, message.Content))
        {
            // If not a command, broadcast the message normally
            Rpc(nameof(S2C_ReceiveMessage), MPacker.Pack(message), MPacker.Pack(World.Instance.GetPlayerById(message.PlayerId).ProfileData));
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void S2C_ReceiveMessage(byte[] messageBytes, byte[] profileBytes)
    {
        ChatMessage message = MPacker.Unpack<ChatMessage>(messageBytes);
        PlayerProfileData senderData = MPacker.Unpack<PlayerProfileData>(profileBytes);
        GD.Print(message.Content);
        OnMessageReceived?.Invoke(message, senderData);
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
        Rpc(nameof(S2C_ReceiveMessage), MPacker.Pack(message), MPacker.Pack(new PlayerProfileData()));
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
        PlayerProfileData serverSystemData = new PlayerProfileData { PlayerName = "SERVER", PlayerColor = Color.FromHtml("#ffff00ff") };
        RpcId(playerId, nameof(S2C_ReceiveMessage), MPacker.Pack(message), MPacker.Pack(serverSystemData));
    }
}
