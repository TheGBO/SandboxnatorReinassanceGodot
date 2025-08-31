using System.IO;

class LoginRequestPacket : Packet
{
    public string GameVersion { get; private set; }

    public LoginRequestPacket(string gameVersion)
    {
        GameVersion = gameVersion;
    }

    public LoginRequestPacket()
    {
        GameVersion = null;
    }
    
    
    public override void Deserialize(BinaryReader r)
    {
        GameVersion = r.ReadString();
    }

    public override void Serialize(BinaryWriter w)
    {
        w.Write(GameVersion);
    }
}