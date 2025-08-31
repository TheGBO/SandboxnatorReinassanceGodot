using System;

public interface ITransport
{
    int LocalPeerId { get; }
    bool IsServer { get; }
    bool IsClient { get; }
    bool IsNetConnected { get; }
    void StartServer(int port);
    void StartHost(int port);
    void SendToLocalHost(byte[] data);
    void StartClient(string host, int port);
    void Stop();
    void PollTransportEvents();

    public void SendPacket<T>(int peerId, T packet, bool reliable) where T : Packet;
    public void BroadCastPacket<T>(T packet, bool reliable) where T : Packet;
    public event Action<long, byte[]> DataReceivedEvent;
    event Action<long> PeerConnectedEvent;
    event Action<long> PeerDisconnectedEvent;
    event Action ClientConnectedToServer;
}
