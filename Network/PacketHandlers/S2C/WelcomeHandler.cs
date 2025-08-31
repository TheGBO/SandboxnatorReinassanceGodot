using Godot;

class WelcomeHandler : IPacketHandler<WelcomePacket>
{
    public static bool WelcomeReceived { get; set; } = false;
    public void Handle(WelcomePacket packet)
    {
        int id = packet.AssignedPlayerId;
        WelcomeReceived = true;
        NetworkManager.Instance.LocalId = id;
        LiteNetLibTransport.Instance.RegisterServerPeer(0);
        GD.Print($"Welcome received from server, assigned ID: {id}");
    }
}