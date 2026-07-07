using FainCraft.Common.Networking.Packets;

namespace FainCraft.Common.Networking.Messages;
public interface INetworkMessage
{
    public Packet Write();
    public abstract static INetworkMessage Read(Packet packet);
}

public static class INetworkMessageExtensions
{
    public static PacketType GetPacketType<T>(this T message) where T : INetworkMessage
    {
        return NetworkMessageRegistry.GetPacketType<T>();
    }
}
