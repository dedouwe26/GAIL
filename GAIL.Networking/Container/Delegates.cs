namespace GAIL.Networking.Client {
    public delegate void PacketCallback(ClientContainer client, Packet packet);
    public delegate void ConnectCallback(ClientContainer client);
    public delegate void StopCallback(ClientContainer client);
    public delegate void PacketSentCallback(ClientContainer client, Packet packet);
}

namespace GAIL.Networking.Server {
    public delegate void PacketCallback(ServerContainer server, Connection connection, Packet packet);
    public delegate void StartCallback(ServerContainer server);
    public delegate void ConnectCallback(ServerContainer server, Connection connection);
    public delegate void StopCallback(ServerContainer server);
    public delegate void PacketSentCallback(ServerContainer server, Connection connection, Packet packet);
}

