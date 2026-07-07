using FainCraft.Common.Networking.Messages;
using FainCraft.Common.Networking.Packets;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FainCraft.Server.Networking;
internal class NetworkClient
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required NetworkServer Server { get; init; }

    public void Send(ReadOnlySpan<byte> data)
    {
        Server.SendToClient(data, Id);
    }

    public void Send(Packet packet)
    {
        Server.SendToClient(packet.Data, Id);
    }

    public void Send(INetworkMessage message)
    {
        var packet = message.Write();
        Server.SendToClient(packet.Data, Id);
    }

    public void BroadcastExcept(ReadOnlySpan<byte> data)
    {
        Server.BroadcastExcept(data, Id);
    }

    public void BroadcastExcept(Packet packet)
    {
        Server.BroadcastExcept(packet.Data, Id);
    }

    public void BroadcastExcept(INetworkMessage message)
    {
        var packet = message.Write();
        Server.BroadcastExcept(packet.Data, Id);
    }
}
