using System;

public interface ITransport
{
    int LocalPeerId { get; }
    bool IsServer { get; }
    bool IsNetConnected { get; }
    void StartServer(int port);
    void StartClient(string host, int port);
    void Stop();

    void Send(long peerId, byte[] data, bool reliable = true);
    void Broadcast(byte[] data, bool reliable = true);
    public event Action<long, byte[]> OnDataReceived;
    event Action<long> OnPeerConnected;
    event Action<long> OnPeerDisconnected;
}
