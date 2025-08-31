using System;
using System.Collections.Generic;
using System.Reflection;

public class PacketHandlerRegistry
{
    private readonly Dictionary<Type, object> handlers = new();

    public void RegisterPacketHandler<TPacket>(IPacketHandler<TPacket> packetHandler) where TPacket : IPacket
    {
        Type packetType = typeof(TPacket);

        if (handlers.ContainsKey(packetType))
        {
            throw new InvalidOperationException($"you can NOT register {packetType.Name} twice vrother... :c </3");
        }

        handlers[packetType] = packetHandler;
    }
    
        public void Handle(long peerId, IPacket packet)
        {
            Type packetType = packet.GetType();
            
            if (handlers.TryGetValue(packetType, out object handler))
            {
                // Use reflection to call the strongly-typed Handle method
                Type handlerInterfaceType = typeof(IPacketHandler<>).MakeGenericType(packetType);
                MethodInfo handleMethod = handlerInterfaceType.GetMethod("Handle");
                
                handleMethod?.Invoke(handler, new object[] { packet });
            }
            else
            {
                throw new InvalidOperationException($"No handler registered for packet type {packetType.Name}");
            }
        }
}