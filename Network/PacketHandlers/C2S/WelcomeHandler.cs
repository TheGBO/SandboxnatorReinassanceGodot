using Godot;

class WelcomeHandler : IPacketHandler<WelcomePacket>
{
    public void Handle(WelcomePacket packet)
    {
        int id = packet.AssignedPlayerId;
        NetworkManager.Instance.LocalId = id;
        GD.Print($"Welcome received from server, assigned ID: {id}");
    }
}