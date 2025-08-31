using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

//Mostly AI generated class because i was too lazy to rework every single line of the networking layer which is already complicated enough.
public partial class LiteNetLibTransport : Singleton<LiteNetLibTransport>, ITransport, INetEventListener
{
    private NetManager netManager;
    private NetPeer serverPeer;
    private bool isServer = false;
    private bool isClient = false;

    public int LocalPeerId { get; private set; } = -1;
    public bool IsServer => isServer;
    public bool IsClient => isClient;
    public bool IsNetConnected => (isServer && netManager != null && netManager.IsRunning) ||
                                  (isClient && serverPeer != null && serverPeer.ConnectionState == ConnectionState.Connected);

    public event Action<long, byte[]> DataReceivedEvent;
    public event Action<long> PeerConnectedEvent;
    public event Action<long> PeerDisconnectedEvent;
    public event Action ClientConnectedToServer;


    private Dictionary<long, NetPeer> connectedClients = new Dictionary<long, NetPeer>();
    private Dictionary<int, long> peerIdToGameId = new();
    private int nextGameId = 1;

    public override void _Process(double delta)
    {
        netManager?.PollEvents();
    }

    public void StartServer(int port)
    {
        netManager = new NetManager(this);
        netManager.Start(port);
        isServer = true;
        isClient = false;
        LocalPeerId = 0; // server ID
        GD.Print($"LiteNetLib server started on port {port}");
    }

    public void StartClient(string host, int port)
    {
        netManager = new NetManager(this);
        isServer = false;
        isClient = true;
        LocalPeerId = 1; // temporary client ID, will use serverPeer.Id after connection
        netManager.Start();
        netManager.Connect(host, port, "Sandboxnator");
        GD.Print($"LiteNetLib client connecting to {host}:{port}");
    }

    public void StartHost(int port)
    {
        // Start server side
        isServer = true;
        isClient = true; // because the host is also a client
        LocalPeerId = 0; // host is server ID

        netManager = new NetManager(this);
        netManager.Start(port);

        SimulateHostConnection();

        GD.Print($"Host started on port {port}");
    }

    private void SimulateHostConnection()
    {
        int hostGameId = 1;
        nextGameId = 2;

        // Register host pseudo-client
        connectedClients[hostGameId] = null;
        peerIdToGameId[-1] = hostGameId;

        // Trigger connection event manually
        PeerConnectedEvent?.Invoke(hostGameId);

        // Build the welcome packet
        var welcomePacket = new WelcomePacket { AssignedPlayerId = hostGameId };

        // Properly serialize the packet with ID first
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter w = new BinaryWriter(ms))
        {
            w.Write(PacketFactory.GetPacketId<WelcomePacket>());
            welcomePacket.Serialize(w);

            // Send to local client
            SendToLocalHost(ms.ToArray());
        }

        GD.Print($"Host client connected with ID: {hostGameId}");
    }





    public void Stop()
    {
        netManager?.Stop();
        netManager = null;
        connectedClients.Clear();
        serverPeer = null;
        isServer = false;
        isClient = false;
        LocalPeerId = -1;
    }

    public void SendPacket<T>(int peerId, T packet, bool reliable) where T : Packet
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            // 1. Write the packet ID first
            writer.Write(PacketFactory.GetPacketId<T>());

            // 2. Serialize the packet data
            packet.Serialize(writer);

            Send(peerId, ms.ToArray(), reliable);
        }
    }

    public void BroadCastPacket<T>(T packet, bool reliable) where T : Packet
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            // 1. Write the packet ID first
            writer.Write(PacketFactory.GetPacketId<T>());

            // 2. Serialize the packet data
            packet.Serialize(writer);

            Broadcast(ms.ToArray(), reliable);
        }
    }

    private void Send(long peerId, byte[] data, bool reliable = true)
    {
        if (isServer)
        {
            if (peerId == 0)
            {
                SendToLocalHost(data);
            }
            else if (connectedClients.TryGetValue(peerId, out var client))
            {
                client.Send(data, reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
            }
        }
        else if (isClient)
        {
            serverPeer?.Send(data, reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
        }
    }

    public void SendToLocalHost(byte[] data)
    {
        DataReceivedEvent?.Invoke(LocalPeerId, data);
    }


    private void Broadcast(byte[] data, bool reliable = true)
    {
        if (!isServer) return;

        SendToLocalHost(data); // host pseudo-client

        foreach (var kv in connectedClients)
        {
            var peer = kv.Value;
            if (peer == null) continue; // skip host pseudo-client
            peer.Send(data, reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
        }
    }


    #region INetEventListener Implementation

    public void OnPeerConnected(NetPeer peer)
    {
        if (isServer)
        {
            int gameId = nextGameId++;
            connectedClients[gameId] = peer;
            peerIdToGameId[peer.Id] = gameId;

            var welcomePacket = new WelcomePacket { AssignedPlayerId = gameId };
            SendPacket(gameId, welcomePacket, true);

            PeerConnectedEvent?.Invoke(gameId);
            GD.Print($"[SERVER] Assigned GameID {gameId} to PeerID {peer.Id}");
        }
        else if (isClient)
        {
            serverPeer = peer;
            GD.Print($"[CLIENT] Connected to server: PeerID={peer.Id}");
        }
    }


    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (isServer)
        {
            connectedClients.Remove(peer.Id);
        }
        else if (isClient)
        {
            serverPeer = null;
        }

        PeerDisconnectedEvent?.Invoke(peer.Id);
        GD.Print($"Peer disconnected: {peer.Id} ({disconnectInfo.Reason})");
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        byte[] data = reader.GetRemainingBytes();
        DataReceivedEvent?.Invoke(peer.Id, data);
        reader.Recycle();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        // Optional: handle server discovery or unconnected messages
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        // Optional: track latency
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        if (!isServer) return;

        // Accept all incoming connections
        request.AcceptIfKey("Sandboxnator");
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        throw new NotImplementedException();
    }

    public void PollTransportEvents()
    {
        netManager.PollEvents();
    }

    #endregion
}
