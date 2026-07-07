using FainCraft.Common.Networking.Packets;

namespace FainCraft.Common.Networking.Messages;
[PacketType(PacketType.ServerDisconnectClient)]
public class ServerDisconnectClientMessage : INetworkMessage
{
    public required string Reason { get; set; }

    public static INetworkMessage Read(Packet packet)
    {
        return new ServerDisconnectClientMessage
        {
            Reason = packet.ReadString()
        };
    }

    public Packet Write()
    {
        var writer = new PacketWriter(this.GetPacketType());
        writer.Write(Reason);
        return writer.Build();
    }
}
