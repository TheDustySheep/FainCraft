namespace FainCraft.Server.Networking;
internal class SignalRecievedHandler
{
    private delegate void SignalHandler(NetworkServer server, NetworkClient client, INetworkSignal signal);
    private readonly Dictionary<Type, SignalHandler> _handlers = new();

    public void Register<T>(Action<T> action) where T : INetworkSignal
    {
        _handlers[typeof(T)] = (server, client, signal) => action((T)signal);
    }

    public void Register<T>(Action<NetworkClient, T> action) where T : INetworkSignal
    {
        _handlers[typeof(T)] = (server, client, signal) => action(client, (T)signal);
    }

    public void Register<T>(Action<NetworkServer, NetworkClient, T> action) where T : INetworkSignal
    {
        _handlers[typeof(T)] = (server, client, signal) => action(server, client, (T)signal);
    }

    public void Handle(NetworkServer server, NetworkClient client, ReadOnlySpan<byte> data)
    {
        INetworkSignal signal = NetworkSignalRegistry.Deserialize(data);

        if (!_handlers.TryGetValue(signal.GetType(), out var handler))
            throw new NotSupportedException($"No handler registered for signal type {signal.GetType()}");
        
        handler(server, client, signal);
    }
}
