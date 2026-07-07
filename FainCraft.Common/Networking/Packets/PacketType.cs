namespace FainCraft.Common.Networking.Packets;
public enum PacketType : byte
{
    // Client
    ClientWelcome = 0,

    // Server
    ServerDisconnectClient = 100
}
