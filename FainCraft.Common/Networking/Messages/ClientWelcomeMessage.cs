using FainCraft.Common.Networking.Packets;

namespace FainCraft.Common.Networking.Messages;
public class ClientWelcomeMessage : INetworkMessage
{
    public static PacketType PacketType => PacketType.ClientWelcome;

    public required string Username { get; init; }

    public static INetworkMessage Read(Packet packet)
    {
        return new ClientWelcomeMessage()
        { 
            Username = packet.ReadString() 
        };
    }

    public Packet Write()
    {
        var writer = new PacketWriter(PacketType);
        writer.Write(Username);
        return writer.Build();
    }
}
