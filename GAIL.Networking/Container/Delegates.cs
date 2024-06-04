namespace GAIL.Networking {
    /// <summary>
    /// An callback for when an exception is thrown (only IOException and SocketException).
    /// </summary>
    /// <param name="exception">The exception that is thrown (only IOException and SocketException).</param>
    public delegate void ExceptionCallback(Exception exception);
}

namespace GAIL.Networking.Client {
    /// <summary>
    /// An callback for when a packet is received.
    /// </summary>
    /// <param name="client">The client that recieved a packet.</param>
    /// <param name="packet">The packet that has been received.</param>
    public delegate void PacketCallback(ClientContainer client, Packet packet);
    /// <summary>
    /// An callback for when a connection is astablished with the server.
    /// </summary>
    /// <param name="client">The connected client.</param>
    public delegate void ConnectCallback(ClientContainer client);
    /// <summary>
    /// An callback for when a connection is astablished with the server.
    /// </summary>
    /// <param name="client">The connected client.</param>
    /// <param name="byServer">True if the server disconnected.</param>
    /// <param name="additionalData">The optional additional data.</param>
    public delegate void DisconnectCallback(ClientContainer client, bool byServer, byte[] additionalData);
    /// <summary>
    /// An callback for when the client has stopped.
    /// </summary>
    /// <param name="client">The client that has stopped.</param>
    public delegate void StopCallback(ClientContainer client);
    /// <summary>
    /// An callback for when a packet has been sent.
    /// </summary>
    /// <param name="client">The client that has sent the packet.</param>
    /// <param name="packet">The packet that has been sent.</param>
    public delegate void PacketSentCallback(ClientContainer client, Packet packet);
}

namespace GAIL.Networking.Server {
    /// <summary>
    /// An callback for when a packet is received.
    /// </summary>
    /// <param name="server">The server that recieved a packet.</param>
    /// <param name="connection">The connection that the packet comes from.</param>
    /// <param name="packet">The packet that has been received.</param>
    public delegate void PacketCallback(ServerContainer server, Connection connection, Packet packet);
    /// <summary>
    /// An callback for when the server started listening.
    /// </summary>
    /// <param name="server">The server that started listening.</param>
    public delegate void StartCallback(ServerContainer server);
    /// <summary>
    /// An callback for when a connection is astablished with the client.
    /// </summary>
    /// <param name="server">The server that has established a connection.</param>
    /// <param name="connection">The connection that is established.</param>
    public delegate void ConnectCallback(ServerContainer server, Connection connection);
    /// <summary>
    /// An callback for when a client has disconnected.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <param name="connection">The disconnected client.</param>
    /// <param name="byClient">True if the client disconnected.</param>
    /// <param name="additionalData">The optional additional data.</param>
    public delegate void DisconnectCallback(ServerContainer server, Connection connection, bool byClient, byte[] additionalData);
    /// <summary>
    /// An callback for when the server stopped listening.
    /// </summary>
    /// <param name="server">The server that stopped.</param>
    public delegate void StopCallback(ServerContainer server);
    /// <summary>
    /// An callback for when the server has sent a packet. 
    /// </summary>
    /// <param name="server">The server that has sent a packet.</param>
    /// <param name="connection">The connection that the packet has been sent to.</param>
    /// <param name="packet">The packet that has been sent.</param>
    public delegate void PacketSentCallback(ServerContainer server, Connection connection, Packet packet);
}

