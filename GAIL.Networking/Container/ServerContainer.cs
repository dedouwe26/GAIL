using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using GAIL.Networking.Parser;

namespace GAIL.Networking.Server;

// TODO: send asynchronous

/// <summary>
/// This is a server that listens for packets.
/// </summary>
public class ServerContainer : IDisposable, IAsyncDisposable {
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

    /// <summary>
    /// If the server is closed.
    /// </summary>
    public bool Closed {get; private set;}

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
    /// <returns>True if it started listening (and no other program on <see cref="IP"/>).</returns>
    public bool Start() {
        try {
            tcpListener.Start();
        } catch (SocketException) {
            return false;
        }
        OnStart?.Invoke(this);
        listenThread = new Thread(Listen);
        listenThread.Start();
        return true;
    }


    #region Send
    
    /// <summary>
    /// Sends a packet to a connection using the ID.
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="connection">The connection to send the packet to.</param>
    public void SendPacket(Packet packet, Connection connection) {
        connection.Stream.Write(PacketParser.FormatPacket(packet));
        connection.Stream.Flush();
        OnPacketSent?.Invoke(this, connection, packet);
    }

    /// <summary>
    /// Sends a packet to a connection using the ID.
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="ID">The ID of the connection to send the packet to.</param>
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
    
    #endregion Send
    
    
    #region Send Asynchronous
    
    /// <summary>
    /// Sends a packet to a connection using the ID (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="connection">The connection to send the packet to.</param>
    public async ValueTask SendPacketAsync(Packet packet, Connection connection) {
        await connection.Stream.WriteAsync(PacketParser.FormatPacket(packet));
        await connection.Stream.FlushAsync();
        OnPacketSent?.Invoke(this, connection, packet);
    }

    /// <summary>
    /// Sends a packet to a connection using the ID (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="ID">The ID of the connection to send the packet to.</param>
    /// <returns>True if an connection has been found, otherwise false.</returns>
    public async ValueTask<bool> SendPacketAsync(Packet packet, byte[] ID) {
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        await SendPacketAsync(packet, connection);
        return true;
    }

    /// <summary>
    /// Broadcasts a packet to all clients (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to broadcast.</param>
    public async ValueTask BroadcastPacketAsync(Packet packet) {
        foreach (Connection connection in connections.Values) {
            await SendPacketAsync(packet, connection);
        }
    }

    #endregion Send Asynchronous


    #region Disconnect

    /// <summary>
    /// Disconnects a connection using an ID.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    public void Disconnect(Connection connection, byte[]? additionalData = null) {
        additionalData ??= [];
        SendPacket(new DisconnectPacket(additionalData), connection);
        connections.Remove(connection.ID);
        connection.Dispose();
        OnDisconnect?.Invoke(this, connection, false, additionalData);
        
    }

    /// <summary>
    /// Disconnects a connection using an ID.
    /// </summary>
    /// <param name="ID">The ID of the connection used to disconnect the client.</param>
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

    #endregion Disconnect


    #region Disconnect Asynchronous

    /// <summary>
    /// Disconnects a connection using an ID (asynchronous).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    public async ValueTask DisconnectAsync(Connection connection, byte[]? additionalData = null) {
        additionalData ??= [];
        await SendPacketAsync(new DisconnectPacket(additionalData), connection);
        connections.Remove(connection.ID);
        connection.Dispose();
        OnDisconnect?.Invoke(this, connection, false, additionalData);
    }

    /// <summary>
    /// Disconnects a connection using an ID (asynchronous).
    /// </summary>
    /// <param name="ID">The ID of the connection used to disconnect the client.</param>
    /// <returns>True if an connection has been found, otherwise false.</returns>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    public async ValueTask<bool> DisconnectAsync(byte[] ID, byte[]? additionalData = null) {
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        await DisconnectAsync(connection, additionalData);
        return true;
    }

    /// <summary>
    /// Kicks or disconnects all clients from this server (asynchronous).
    /// </summary>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    public async ValueTask DisconnectAllAsync(byte[]? additionalData = null) {
        foreach (Connection connection in connections.Values) {
            await DisconnectAsync(connection, additionalData);
        }
    }

    #endregion Disconnect Asynchronous


    private void Listen() {
        tcpListener.BeginAcceptTcpClient(ListenConnection, null);
        tcpListener.EndAcceptTcpClient()
        while (!Closed) {
            
            Connection connection = new(tcpListener.AcceptTcpClient());
            connections.Add(connection.ID, connection);
            OnConnect?.Invoke(this, connection);
            ThreadPool.QueueUserWorkItem(ListenConnection, connection);
        }
    }

    private void ListenConnection(IAsyncResult result)
    {
        throw new NotImplementedException();
    }

    private void ListenConnection(object? state) {
        Connection connection = (Connection)state!;

        try {
            PacketParser.Parse(connection.Stream, () => Closed, (Packet p) => {
                OnPacket?.Invoke(this, connection, p);
                if (p is DisconnectPacket) {
                    connections.Remove(connection.ID);
                    connection.Dispose();
                    OnDisconnect?.Invoke(this, connection, true, (p as DisconnectPacket)!.AdditionalData);
                    return true;
                }
                return false;
            });
        } catch (IOException) {
            
        }
        
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
        Closed = true;
        tcpListener.Stop();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
    /// <summary>
    /// Stops listening and disconnects all clients (asynchronous).
    /// </summary>
    public async ValueTask StopAsync() {
        await DisposeAsync();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops listening and disconnects all clients (asynchronous).
    /// </remarks>
    public async ValueTask DisposeAsync() {
        await DisconnectAllAsync();
        Closed = true;
        tcpListener.Stop();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}