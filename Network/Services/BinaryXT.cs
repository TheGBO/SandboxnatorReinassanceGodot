using Godot;
using System;
using System.IO;

public static class BinaryXT
{
    public static void WriteVec3(this BinaryWriter w, Vector3 vec)
    {
        w.Write(vec.X);
        w.Write(vec.Y);
        w.Write(vec.Z);
    }

    public static Vector3 ReadVec3(this BinaryReader r)
    {
        float x = r.ReadSingle();
        float y = r.ReadSingle();
        float z = r.ReadSingle();
        return new Vector3(x, y, z);
    }

    public static void WriteColor(this BinaryWriter w, Color color)
    {
        w.Write(color.R);
        w.Write(color.G);
        w.Write(color.B);
    }

    public static Color ReadColor(this BinaryReader r)
    {
        float red = r.ReadSingle();
        float green = r.ReadSingle();
        float blue = r.ReadSingle();
        return new Color(red, green, blue);
    }

    public static void WritePlayerProfileData(this BinaryWriter w, PlayerProfileData pData)
    {
        w.Write(pData.PlayerName);
        w.WriteColor(pData.PlayerColor);
    }

    public static PlayerProfileData ReadPlayerProfileData(this BinaryReader r)
    {
        string pName = r.ReadString();
        Color pColor = r.ReadColor();
        return new PlayerProfileData { PlayerName = pName, PlayerColor = pColor };
    }

    public static void WriteQuaternion(this BinaryWriter w, Quaternion q)
    {
        w.Write(q.X);
        w.Write(q.Y);
        w.Write(q.Z);
        w.Write(q.W);
    }

    public static Quaternion ReadQuaternion(this BinaryReader r)
    {
        float x = r.ReadSingle();
        float y = r.ReadSingle();
        float z = r.ReadSingle();
        float w = r.ReadSingle();
        return new Quaternion(x, y, z, w);
    }
}