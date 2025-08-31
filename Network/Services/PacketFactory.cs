using System;
using System.Collections.Generic;
using Godot;

public static class PacketFactory
{
    // Static registry for type mapping - only one instance for Type across entire app
    private static readonly Dictionary<Type, ushort> _typeToId = new();

    public static void Register<TPacket>(ushort id) where TPacket : IPacket, new()
    {
        Type packetType = typeof(TPacket);
        string registryId = id.ToString();

        if (Registry<Type>.Contains(registryId)) return;

        Registry<Type>.Register(registryId, packetType);
        GD.Print($"Registered packet {registryId}:{packetType}");
        _typeToId[packetType] = id;
    }

    public static IPacket CreatePacket(ushort id)
    {
        string registryId = id.ToString();
        Type packetType = Registry<Type>.Get(registryId);
        return (IPacket)Activator.CreateInstance(packetType);
    }

    public static ushort GetPacketId<TPacket>() where TPacket : IPacket
    {
        if (_typeToId.TryGetValue(typeof(TPacket), out ushort id))
        {
            return id;
        }
        throw new Exception($"Packet type {typeof(TPacket).Name} is not registered");
    }

    public static bool IsRegistered<TPacket>() where TPacket : IPacket
    {
        return _typeToId.ContainsKey(typeof(TPacket));
    }

    // Clear all registrations (important for Godot hot-reload)
    public static void Clear()
    {
        // You'd need to add a Clear method to your Registry<T> class
        // Registry<Type>.Clear();
        _typeToId.Clear();
    }
}