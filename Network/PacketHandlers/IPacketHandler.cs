// TPacket is a placeholder for a specific packet type (e.g., LoginPacket, ChatPacket)
// The "where" clause constrains TPacket to only be types that implement IPacket.
public interface IPacketHandler<TPacket> where TPacket : IPacket
{
    // The method now expects the SPECIFIC type, not the general interface.
    void Handle(TPacket packet);
}