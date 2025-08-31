using Godot;

class SpawnPlayerHandler : IPacketHandler<SpawnPlayerPacket>
{
    public void Handle(SpawnPlayerPacket packet)
    {
        // Only spawn if not already present
        if (PlayerManager.Instance.HasEntity(packet.PlayerId))
            return;

        Player playerInstance = PlayerManager.Instance.SpawnPlayer(
            packet.PlayerId,
            packet.PlayerPosition,
            packet.PlayerRotation
        );
    }
}
