using FainCraft.Server;

Console.WriteLine("Hello, World!");

// Server Setup
var server = new GameServer();

// NetworkClient Setup
var tcpClient = new SimpleTCP.SimpleTcpClient();
tcpClient.Connect("localhost", 5000);

while (true)
{
    Console.WriteLine("Enter Name: ");
    //var signal = new ClientConnectMessage() { PlayerName = Console.ReadLine()! };
    //tcpClient.TcpClient.GetStream().Write(NetworkSignalRegistry.Serialize(signal));
    Thread.Sleep(1000);
}

