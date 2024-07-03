using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using GAIL.Networking.Parser;
using OxDED.Terminal;
using OxDED.Terminal.Logging;

namespace GAIL.Networking.Server;

/// <summary>
/// This is a server that listens for packets.
/// </summary>
public class ServerContainer : IDisposable, IAsyncDisposable {
    /// <summary>
    /// Creates a server.
    /// </summary>
    /// <param name="ip">The IP end point to listen on (IP address and port).</param>
    /// <param name="enableLogging">If the server should start logging.</param>
    /// <param name="logger">The logger to use (default: ID: GAIL.Networking.Server.{ Connection.CreateID( IP ) } ).</param>
    /// <returns>The created server.</returns>
    public static ServerContainer Create(IPEndPoint ip, bool enableLogging = false, Logger? logger = null) {
        ServerContainer server = new(ip);
        if (enableLogging) {
            server.SetLogger(logger);
        }
        return server;
    }
    /// <summary>
    /// The logger of this server.
    /// Set it using <see cref="SetLogger"/>
    /// </summary>
    public Logger? Logger { get; private set; }
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
    /// An event for when an exception is thrown (only IOException, SocketException).
    /// </summary>
    public event ExceptionCallback? OnException;

    /// <summary>
    /// If the server is closed.
    /// </summary>
    public bool Closed {get; private set;}
    private ServerContainer(IPEndPoint ip) {
        Closed = true;
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
    /// Starts the server (doesn't block the thread).
    /// </summary>
    /// <returns>True if it started listening (and no other program on <see cref="IP"/>).</returns>
    public bool Start() {
        try {
            tcpListener.Start();
        } catch (SocketException e) {
            Logger?.LogFatal("Unable to start listening: '"+e.Message+"'.");
            OnException?.Invoke(e, null);
            return false;
        }
        Closed = false;
        OnStart?.Invoke(this);
        Listen();
        return true;
    }

    /// <summary>
    /// Sets the logger of this server.
    /// </summary>
    /// <param name="logger">The logger for this server ( default: ID: GAIL.Networking.Server.{ Connection.CreateID( IP ) } ).</param>
    /// <param name="disable">If it should disable the logging.</param>
    public void SetLogger(Logger? logger = null, bool disable = false) {
        Logger = disable ? null : (logger ?? new Logger(
            $"GAIL.Networking.Server.{Connection.CreateID(IP)}",
            "Networking Server",
            Severity.Info,
            new () {
                [typeof(TerminalTarget)] = new TerminalTarget()
            }
        ));
        
        if (Logger?.HasTarget(typeof(TerminalTarget)) ?? false) {
            Logger.GetTarget<TerminalTarget>().Format = "<{0}>: ("+Color.DarkBlue.ToForegroundANSI()+"{2}"+ANSI.Styles.ResetAll+")[{5}"+ANSI.Styles.Bold+"{3}"+ANSI.Styles.ResetAll+"] : {5}{4}"+ANSI.Styles.ResetAll;
            Logger.GetTarget<TerminalTarget>().NameFormat =  "{0} - {1}";
        }
        if (Logger?.HasTarget(typeof(FileTarget)) ?? false) {
            Logger.GetTarget<FileTarget>().Format = "<{0}>: ({2})[{3}] : {4}";
            Logger.GetTarget<FileTarget>().NameFormat =  "{0} - {1}";
        }
    }

    #region Send
    
    /// <summary>
    /// Sends a packet to a connection using the ID.
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="connection">The connection to send the packet to.</param>
    public void SendPacket(Packet packet, Connection connection) { // TODO: add exception handling at ALL send methods.
        if (Closed) { return; }
        try {
            NetworkParser.Serialize(connection.Stream, packet);
        } catch (IOException e) {
            Logger?.LogError($"Could not send packet (connection ID: {BitConverter.ToString(connection.ID).Replace("-", "")}): '{e.Message}'.");
            OnException?.Invoke(e, connection);
        }
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
        if (Closed) { return false; }
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
        if (Closed) { return; }
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
        if (Closed) { return; }
        try {
            NetworkParser.Serialize(connection.Stream, packet); // TODO?: this isnt really async.
        } catch (IOException e) {
            Logger?.LogError($"Could not send packet (connection ID: {BitConverter.ToString(connection.ID).Replace("-", "")}): '{e.Message}'.");
            OnException?.Invoke(e, connection);
        }
        
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
        if (Closed) { return false; }
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
        if (Closed) { return; }
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
        if (Closed) { return; }
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
        if (Closed) { return false; }
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
        if (Closed) { return; }
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
        if (Closed) { return; }
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
        if (Closed) { return false; }
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
        if (Closed) { return; }
        foreach (Connection connection in connections.Values) {
            await DisconnectAsync(connection, additionalData);
        }
    }

    #endregion Disconnect Asynchronous


    private void Listen() {
        if (Closed) { return; }
        tcpListener.BeginAcceptTcpClient(new(ListenConnection), null);
    }

    private void ListenConnection(IAsyncResult result) {
        if (Closed) {return;}

        Connection connection = new(tcpListener.EndAcceptTcpClient(result));

        // Dispatch new.
        Listen();
        
        connections.Add(connection.ID, connection);
        OnConnect?.Invoke(this, connection);
        try {
            NetworkParser.Parse(connection.Stream, () => Closed || connection.Closed, (Packet p) => { // TODO: handle return value.
                OnPacket?.Invoke(this, connection, p);
                if (p is DisconnectPacket) {
                    connections.Remove(connection.ID);
                    connection.Dispose();
                    OnDisconnect?.Invoke(this, connection, true, (p as DisconnectPacket)!.AdditionalData);
                    return true;
                }
                return false;
            });
        } catch (IOException e) { // FIXME: // FIXME: when stopping (from server).
            if (Closed || connection.Closed) {
                return;
            }
            Logger?.LogWarning($"Could not read from network stream (connection ID: {BitConverter.ToString(connection.ID).Replace("-", "")}): '{e.Message}'.");
            OnException?.Invoke(e, connection);
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
        if (Closed) { return; }
        Logger?.LogDebug("Disposing");
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
        if (Closed) { return; }
        await DisconnectAllAsync();
        Closed = true;
        tcpListener.Stop();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}