using Godot;
using System;
using System.Collections.Generic;
using NullGarel.Util;
using NullGarel.Sandboxnator.Network;
using NullGarel.Sandboxnator.WorldAndScenes;
using NullGarel.Sandboxnator.Entity;
using NullGarel.Sandboxnator.Commands;
using NullGarel.Util.Log;
using NullGarel.Util.GodotHelpers;

namespace NullGarel.Sandboxnator.Chat;

/// <summary>
/// Singleton responsible for sending, receiving, parsing and handling chat messages and commands.
/// </summary>
public partial class ChatManager : Singleton<ChatManager>
{
    public Action<ChatMessage, PlayerProfileData> OnMessageReceived;

    public override void _Ready()
    {
        // this component should be authority of the server.
        SetMultiplayerAuthority(1);
    }

    /// <summary>
    /// Called on client to request sending a message to the server.
    /// </summary>
    public void RequestSendMessageToServer(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return;

        RpcId(1, nameof(ServerBoundChatMessage), msg);
    }

    /// <summary>
    /// RPC sent from a client to the server to process a chat message.
    /// </summary>
    /// <param name="content"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void ServerBoundChatMessage(string content)
    {
        if (!Multiplayer.IsServer()) return;

        long senderPeerId = Multiplayer.GetRemoteSenderId();
        Player sender = SandboxnatorMain.World.GetPlayerById((int)senderPeerId);

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
            Rpc(nameof(ClientBoundChatMessage), content, (int)senderPeerId);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void ClientBoundChatMessage(string content, int senderPeerId)
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
            Player sender = SandboxnatorMain.World.GetPlayerById(senderPeerId);
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

        Rpc(nameof(ClientBoundChatMessage), msg, -1);
    }

    /// <summary>
    /// SERVER SIDE: Sends a private message to a single player without an attached player.
    /// </summary>
    public void SendPlayerlessMessage(string msg, int recipientPeerId)
    {
        if (!Multiplayer.IsServer())
            throw new InvalidOperationException("This operation can only be called on the server.");

        RpcId(recipientPeerId, nameof(ClientBoundChatMessage), msg, -1);
    }
}