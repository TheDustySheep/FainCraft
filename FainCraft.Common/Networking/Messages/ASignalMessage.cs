using FainCraft.Common.Networking.Packets;

namespace FainCraft.Common.Networking.Messages;
public abstract class ASignalMessage<T> : INetworkMessage where T : ASignalMessage<T>, new()
{
    public static INetworkMessage Read(Packet packet) => new T();
    public Packet Write() => new Packet(NetworkMessageRegistry.GetPacketType<T>());
}
