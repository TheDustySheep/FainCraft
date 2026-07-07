using System.Net.Sockets;
using FainCraft.Common.Networking.Messages;
using FainCraft.Common.Networking.Packets;
using SimpleTCP;

namespace FainCraft.Server.Networking;
internal class NetworkServer
{
    private SimpleTcpServer _server;

    private Dictionary<Guid, TcpClient> _tcpclients = new();
    private Dictionary<Guid, NetworkClient> _networkClients = new();

    public int MaxClients;
    public int TotalClients => _tcpclients.Count;

    public NetworkServer(int maxClients=16, int port=5000)
    {
        MaxClients = maxClients;

        _server = new SimpleTcpServer();
        _server.ClientConnected += ClientConnect;
        _server.ClientDisconnected += ClientDisconnect;
        _server.DataReceived += DataReceived;

        _server.Start(port);
        Console.WriteLine($"Started server on port: {port}");
    }

    ~NetworkServer() => _server.Stop();

    private void DisconnectClient(Guid id, string reason)
    {
        if (!_tcpclients.TryGetValue(id, out var client))
            return;

        SendToClient(id, new ServerDisconnectClientMessage() { Reason = reason });
        _tcpclients.Remove(id);

        client.Client.Shutdown(SocketShutdown.Both);
        client.Dispose();
    }

    private void ClientConnect(object? sender, TcpClient e)
    {    
        void DisconnectClient(string reason)
        {
            var message = new ServerDisconnectClientMessage() { Reason = reason };
            var data = message.Write().Data;
            e.Client.Shutdown(SocketShutdown.Both);
            e.Dispose();
        }

        Guid id = Guid.NewGuid();
        _tcpclients[id] = e;

        if (ActiveClients >= MaxClients)
            DisconnectClient(id, "Server is full");

        // Auto disconnect client if not accepted within 5 seconds
        Task.Run(async () =>
        {
            await Task.Delay(5_000);
            if (!_networkClients.ContainsKey(id))
                return;

            DisconnectClient(id, "Welcome details timed out");
        });

    }

    private void ClientDisconnect(object? sender, TcpClient e)
    {
        Console.WriteLine($"NetworkClient disconnected from server: {e.Client.RemoteEndPoint}");

        if (!_tcpclients.Remove(e, out var client))
            return;

        ClientConnected?.Invoke(client);
    }

    private void DataReceived(object? sender, Message e)
    {
        Console.WriteLine($"NetworkClient sent data to server: {e.TcpClient.Client.RemoteEndPoint}");

        if (!_tcpclients.TryGetValue(e.TcpClient, out var client))
            throw new Exception("Data recieved from unknown client");

        SignalRecieved?.Invoke(this, client, e.Data);
    }

    #region Sending Data to Clients
    public void SendToClient(Guid id, INetworkMessage packet) => SendToClient(id, packet.Write().Data);
    public void SendToClient(Guid id, Packet packet) => SendToClient(id, packet.Data);
    public void SendToClient(Guid id, ReadOnlySpan<byte> data)
    {
        var tcpClient = _tcpclients[id];
        if (tcpClient.Connected)
            tcpClient.GetStream().Write(data);
    }

    public void Broadcast(INetworkMessage packet) => Broadcast(packet.Write().Data);
    public void Broadcast(Packet packet) => Broadcast(packet.Data);
    public void Broadcast(ReadOnlySpan<byte> data)
    {
        foreach ((var id, var client) in _networkClients)
        {
            var tcpClient = _tcpclients[id];
            if (tcpClient.Connected)
                tcpClient.GetStream().Write(data);
        }
    }

    public void BroadcastExcept(Guid id, INetworkMessage packet) => BroadcastExcept(id, packet.Write().Data);
    public void BroadcastExcept(Guid id, Packet packet) => BroadcastExcept(id, packet.Data);
    public void BroadcastExcept(Guid id, ReadOnlySpan<byte> data)
    {
        foreach (var client in _networkClients)
        {
            if (client.Key == id)
                continue;

            var tcpClient = _tcpclients[id];
            if (tcpClient.Connected)
                tcpClient.GetStream().Write(data);
        }
    }
    #endregion
}