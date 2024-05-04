using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace GAIL.Networking.Server;

// TODO: send asynchronous

/// <summary>
/// This is a server that listens for packets.
/// </summary>
public class ServerContainer : IDisposable {
    /// <summary>
    /// Creates a server.
    /// </summary>
    /// <param name="ip">The IP end point to listen for (IP address and port).</param>
    /// <returns>The created server.</returns>
    public static ServerContainer Create(IPEndPoint ip) {
        return new(ip);
    }
    /// <summary>
    /// The IP end point that the server is listening on (IP address and port).
    /// </summary>
    public readonly IPEndPoint IP;
    /// <summary>
    /// All the connections of this server.
    /// </summary>
    public ReadOnlyDictionary<byte[], Connection> Connections { get { return connections.AsReadOnly(); } }
    private readonly Dictionary<byte[], Connection> connections = [];
    /// <summary>
    /// The tcp listener back-end.
    /// </summary>
    public TcpListener tcpListener;
    /// <summary>
    /// An event that is called when a packet is received.
    /// </summary>
    public event PacketCallback? OnPacket;
    /// <summary>
    /// An event that is called when the server has started.
    /// </summary>
    public event StartCallback? OnStart;
    /// <summary>
    /// An event that is called when a connection is astablished.
    /// </summary>
    public event ConnectCallback? OnConnect;
    /// <summary>
    /// An event that is called when a client is disconnected.
    /// </summary>
    public event DisconnectCallback? OnDisconnect;
    /// <summary>
    /// An event that is called when the server has stopped.
    /// </summary>
    public event StopCallback? OnStop;
    /// <summary>
    /// An event that is called when a packet is sent.
    /// </summary>
    public event PacketSentCallback? OnPacketSent;

    private Thread? listenThread;
    private ServerContainer(IPEndPoint ip) {
        IP = ip;
        tcpListener = new TcpListener(ip);
        
    }

    /// <summary>
    /// Disposes the server.
    /// </summary>
    ~ServerContainer() {
        Dispose();
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start() {
        tcpListener.Start();
        OnStart?.Invoke(this);
        listenThread = new Thread(Listen);
        listenThread.Start();
    }
    public void SendPacket(Packet packet, Connection connection) {
        // TODO: disconnect client.
        OnPacketSent?.Invoke(this, connection, packet);
    }
    /// <summary>
    /// Sends a packet to a connection using the ID.
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="ID">The ID used to send a packet.</param>
    /// <returns>True if an connection has been found, otherwise false.</returns>
    public bool SendPacket(Packet packet, byte[] ID) {
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        SendPacket(packet, connection);
        return true;
    }
    /// <summary>
    /// Broadcasts a packet to all clients.
    /// </summary>
    /// <param name="packet">The packet to broadcast.</param>
    public void BroadcastPacket(Packet packet) {
        foreach (Connection connection in connections.Values) {
            SendPacket(packet, connection);
        }
    }
    public void Disconnect(Connection connection) {
        // TODO: disconnect client.
        OnDisconnect?.Invoke(this, connection, false);
        connections.Remove(connection.ID);
        connection.Dispose();
    }
    /// <summary>
    /// Disconnects a connection using an ID.
    /// </summary>
    /// <param name="ID">The ID used to disconnect the client.</param>
    /// <returns>True if an connection has been found, otherwise false.</returns>
    public bool Disconnect(byte[] ID) {
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        Disconnect(connection);
        return true;
    }
    /// <summary>
    /// Kicks or disconnects all clients from this server.
    /// </summary>
    public void DisconnectAll() {
        foreach (Connection connection in connections.Values) {
            Disconnect(connection);
        }
    }
    private void Listen() {
        // TODO: listen for packets
    }
    /// <summary>
    /// Stops listening and disconnects all clients.
    /// </summary>
    public void Stop() {
        Dispose();
    }
    /// <inheritdoc/>
    /// <remarks>
    /// Stops listening and disconnects all clients.
    /// </remarks>
    public void Dispose() {
        DisconnectAll();
        tcpListener.Stop();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}