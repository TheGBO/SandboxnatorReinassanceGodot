using Godot;

class LoginRequestHandler : IPacketHandler<LoginRequestPacket>
{
    public void Handle(LoginRequestPacket packet)
    {
        GD.Print("Packet handling attempt acknowledged!");
    }
}