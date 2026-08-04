using Godot;
using System;
using System.Collections.Generic;
using NullCyan.Util;
using NullCyan.Sandboxnator.Network;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Commands;
using NullCyan.Util.Log;
using NullCyan.Util.GodotHelpers;

namespace NullCyan.Sandboxnator.Chat;

/// <summary>
/// Singleton responsible for sending, receiving, parsing and handling chat messages and commands.
/// </summary>
public partial class ChatManager : Singleton<ChatManager>
{
    public Action<ChatMessage, PlayerProfileData> OnMessageReceived;

    /// <summary>
    /// Called on client to request sending a message to the server.
    /// </summary>
    public void RequestSendMessageToServer(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return;

        RpcId(1, nameof(C2S_HandleMessage), msg);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void C2S_HandleMessage(string content)
    {
        if (!Multiplayer.IsServer()) return;

        long senderPeerId = Multiplayer.GetRemoteSenderId();
        Player sender = World.Instance.GetPlayerById((int)senderPeerId);

        if (sender == null)
        {
            NcLogger.Log($"[SERVER] Chat message rejected: Peer {senderPeerId} not found.");
            return;
        }

        NcLogger.Log($"[AS SERVER] message received: {content.Replace("\n", "|CR+LF|")} from {senderPeerId}");

        // Pass message to command system first
        if (!CommandRegistryManager.ExecuteCommand(sender, content))
        {
            // If not a command, broadcast to all clients with the sender's peer ID
            Rpc(nameof(S2C_ReceiveMessage), content, (int)senderPeerId);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void S2C_ReceiveMessage(string content, int senderPeerId)
    {
        ChatMessage message = new(content, senderPeerId);
        PlayerProfileData senderProfile = null;

        if (senderPeerId == -1)
        {
            senderProfile = new PlayerProfileData()
            {
                PlayerName = "SERVER",
                PlayerColor = Color.FromHtml("#ffff00ff")
            };
        }
        else
        {
            Player sender = World.Instance.GetPlayerById(senderPeerId);
            if (sender != null)
            {
                senderProfile = sender.ProfileData;
            }
        }

        OnMessageReceived?.Invoke(message, senderProfile);
    }

    /// <summary>
    /// SERVER SIDE: send message from server to all clients without an attached player.
    /// </summary>
    public void BroadcastPlayerlessMessage(string msg)
    {
        if (!Multiplayer.IsServer())
            throw new InvalidOperationException("This operation can only be called on the server.");

        Rpc(nameof(S2C_ReceiveMessage), msg, -1);
    }

    /// <summary>
    /// SERVER SIDE: Sends a private message to a single player without an attached player.
    /// </summary>
    public void SendPlayerlessMessage(string msg, int recipientPeerId)
    {
        if (!Multiplayer.IsServer())
            throw new InvalidOperationException("This operation can only be called on the server.");

        RpcId(recipientPeerId, nameof(S2C_ReceiveMessage), msg, -1);
    }
}