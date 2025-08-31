using System.IO;

public interface IPacket
{
    void Serialize(BinaryWriter w);
    void Deserialize(BinaryReader r);
}

