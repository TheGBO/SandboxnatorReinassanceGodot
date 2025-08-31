using System.IO;

public class WelcomePacket : Packet
{
    public int AssignedPlayerId { get; set; }

    public override void Deserialize(BinaryReader r)
    {
        AssignedPlayerId = r.ReadInt32();
    }

    public override void Serialize(BinaryWriter w)
    {
        w.Write(AssignedPlayerId);
    }
}