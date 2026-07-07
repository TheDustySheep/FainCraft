using FainCraft.Common.Networking.Packets;
using System.Reflection;

namespace FainCraft.Common.Networking.Messages;

public static class NetworkMessageRegistry
{
    private delegate INetworkMessage MessageFactory(Packet packet);
    private static readonly Dictionary<PacketType, MessageFactory> _factories = new();
    private static readonly Dictionary<Type, PacketType> _packetTypes = new();

    static NetworkMessageRegistry()
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                typeof(INetworkMessage).IsAssignableFrom(t));

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<PacketTypeAttribute>();
            if (attr == null)
                throw new Exception($"Type {type.FullName} is missing PacketTypeAttribute.");

            var packetType = attr.PacketType;
            if (_factories.ContainsKey(packetType))
                throw new InvalidOperationException($"Factory for {packetType} is already registered.");

            if (_factories.ContainsKey(packetType))
                throw new InvalidOperationException($"Factory for {packetType} is already registered.");

            var method = typeof(NetworkMessageRegistry)
                .GetMethod(nameof(Register))
                !.MakeGenericMethod(type); 
            
            method.Invoke(null, [packetType]);

            _packetTypes[type] = packetType;
        }
    }

    private static void Register<T>(PacketType packetType) where T : INetworkMessage
    {
        _factories[packetType] = T.Read;
    }

    public static INetworkMessage ReadMessage(Packet packet)
    {
        if (!_factories.TryGetValue(packet.Type, out var factory))
            throw new KeyNotFoundException($"No factory registered for {packet.Type}");

        return factory(packet);
    }

    public static PacketType GetPacketType<T>() where T : INetworkMessage
    {
        var type = typeof(T);
        if (!_packetTypes.TryGetValue(type, out var packetType))
            throw new KeyNotFoundException($"No packet type registered for {type.FullName}");
        return packetType;
    }

    public static PacketType GetPacketType(Type type)
    {
        if (!_packetTypes.TryGetValue(type, out var packetType))
            throw new KeyNotFoundException($"No packet type registered for {type.FullName}");
        return packetType;
    }
}