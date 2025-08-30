using System;

public interface ITransport
{
    long LocalPeerId { get; }
    bool IsServer { get; }
    void StartServer(int port);
    void StartClient(string host, int port);
    void Stop();

    void Send(long peerId, byte[] data, bool reliable = true);
    void Broadcast(byte[] data, bool reliable = true, long? exceptPeer = null);

    event Action<long, byte[]> OnReceive;   // (senderId, payload)
    event Action<long> OnPeerConnected;
    event Action<long> OnPeerDisconnected;
}
