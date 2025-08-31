using System.IO;
using Godot;

public class SpawnPlayerPacket : Packet
{
    public int PlayerId { get; set; }
    public Vector3 PlayerPosition { get; set; }
    public Quaternion PlayerRotation { get; set; }

    public override void Deserialize(BinaryReader r)
    {
        PlayerId = r.ReadInt32();
        PlayerPosition = r.ReadVec3();
        PlayerRotation = r.ReadQuaternion();
    }

    public override void Serialize(BinaryWriter w)
    {
        w.Write(PlayerId);
        w.WriteVec3(PlayerPosition);
        w.WriteQuaternion(PlayerRotation);
    }
}