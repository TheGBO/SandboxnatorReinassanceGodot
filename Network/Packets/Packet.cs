using System.IO;

public abstract class Packet : IPacket
{
    public static void RegisterPacket<T>() where T : Packet, new()
    {
        Registry<Packet>.Register(typeof(T).Name, new T());
    }


    public abstract void Deserialize(BinaryReader r);

    public abstract void Serialize(BinaryWriter w);
}