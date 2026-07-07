using FainCraft.Common.Networking.Messages;
using FainCraft.Server.Networking;
namespace FainCraft.Server;
internal class GameServer
{
    private NetworkServer _networkServer;
    private SignalRecievedHandler _handler;
    private Dictionary<Guid, PlayerDetails> _players = new();

    public GameServer(int port=5000)
    {
        _handler = new SignalRecievedHandler();
        //_handler.Register<ClientConnectMessage>(OnClientConnected);
        _networkServer = new NetworkServer(port);
    }

    private void OnConnect(NetworkClient client)
    {
        // client.Send(new ServerRequestClientDetailsSignal());
    }
}
