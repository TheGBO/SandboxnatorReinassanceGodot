using System.IO;

public abstract class Packet : IPacket
{
    public static void RegisterPacket<T>() where T : Packet, new()
    {
        Registry<Packet>.Register(typeof(T).Name, new T());
    }


    public void Deserialize(BinaryReader r)
    {
        throw new System.NotImplementedException();
    }

    public void Serialize(BinaryWriter w)
    {
        throw new System.NotImplementedException();
    }
}