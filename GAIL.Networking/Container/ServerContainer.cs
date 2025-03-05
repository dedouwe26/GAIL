using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using GAIL.Serializing.Formatters;
using OxDED.Terminal;
using OxDED.Terminal.Logging;
using OxDED.Terminal.Logging.Targets;

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
    /// The formatter used to encode / decode all packets.
    /// </summary>
    public IFormatter GlobalFormatter = new DefaultFormatter();

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
    /// An event that is called when a client is safely disconnected.
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
    /// An event for when an exception is thrown.
    /// </summary>
    /// <remarks>
    /// As a result, the connection might be removed.
    /// </remarks>
    public event ExceptionCallback? OnException;

    /// <summary>
    /// If the server is closed.
    /// </summary>
    public bool Closed {get; private set;}
    private ServerContainer(IPEndPoint ip, bool enableLogging = false, Logger? logger = null) {
        Closed = true;
        IP = ip;

        if (enableLogging) {
            SetLogger(logger);
        }

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
            Logger?.LogFatal("Unable to start listening:");
            Logger?.LogException(e, Severity.Fatal);
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
    /// <param name="logger">The logger for this server (on default only contains a terminal target).</param>
    public void SetLogger(Logger? logger = null) {
        Logger = logger ?? new Logger(
            name: "Networking Server",
            severity: Severity.Info,
            targets: [
                new TerminalTarget()
            ]
        );
        int index = Logger.GetTargetIndex<TerminalTarget>();
        if (index > -1) {
            Logger.GetTarget<TerminalTarget>(index)!.Format = "<{0}>: ("+Color.DarkBlue.ToForegroundANSI()+"{2}"+ANSI.Styles.ResetAll+")[{5}"+ANSI.Styles.Bold+"{3}"+ANSI.Styles.ResetAll+"] : {5}{4}"+ANSI.Styles.ResetAll;
            Logger.GetTarget<TerminalTarget>(index)!.NameFormat =  "{0} - {1}";
        }
        index = Logger.GetTargetIndex<FileTarget>();
        if (index > -1) {
            Logger.GetTarget<FileTarget>(index)!.Format = "<{0}>: ({2})[{3}] : {4}";
            Logger.GetTarget<FileTarget>(index)!.NameFormat =  "{0} - {1}";
        }
    }

    #region Send
    
    /// <summary>
    /// Sends a packet to a connection using the ID.
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="connection">The connection to send the packet to.</param>
    /// <returns>True if it succeeded, false if the server is closed or the server could not send the packet.</returns>
    public bool SendPacket(Packet packet, Connection connection) {
        if (Closed) { return false; }
        try {
            connection.Serializer.WritePacket(packet, GlobalFormatter);
        } catch (IOException e) {
            Logger?.LogError($"Could not send packet (connection ID: {connection.ToConnectionID()}):");
            Logger?.LogException(e, Severity.Error);
            OnException?.Invoke(e, connection);
            connections.Remove(connection.ID);
            connection.Dispose();
            return false;
        }
        connection.Stream.Flush();
        OnPacketSent?.Invoke(this, connection, packet);
        return true;
    }

    /// <summary>
    /// Sends a packet to a connection using the ID.
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="ID">The ID of the connection to send the packet to.</param>
    /// <returns>True if an connection has been found and the sending the packet succeeded, otherwise false.</returns>
    public bool SendPacket(Packet packet, byte[] ID) {
        if (Closed) { return false; }
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        return SendPacket(packet, connection);
    }

    /// <summary>
    /// Broadcasts a packet to all clients.
    /// </summary>
    /// <param name="packet">The packet to broadcast.</param>
    /// <returns>True if the server is still running, otherwise false.</returns>
    public bool BroadcastPacket(Packet packet) {
        if (Closed) { return false; }
        foreach (Connection connection in connections.Values) {
            SendPacket(packet, connection);
        }
        return true;
    }
    
    #endregion Send
    
    
    #region Send Asynchronous
    
    /// <summary>
    /// Sends a packet to a connection using the ID (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="connection">The connection to send the packet to.</param>
    /// <returns>True if it succeeded, false if the server is closed or the server could not send the packet.</returns>
    public async ValueTask<bool> SendPacketAsync(Packet packet, Connection connection) {
        if (Closed) { return false; }
        try {
            connection.Serializer.WritePacket(packet, GlobalFormatter); // TODO?: this isnt really async.
        } catch (IOException e) {
            Logger?.LogError($"Could not send packet (connection ID: {connection.ToConnectionID()}):");
            Logger?.LogException(e, Severity.Error);
            OnException?.Invoke(e, connection);
            connections.Remove(connection.ID);
            connection.Dispose();
            return false;
        }
        
        await connection.Stream.FlushAsync();
        OnPacketSent?.Invoke(this, connection, packet);
        return true;
    }

    /// <summary>
    /// Sends a packet to a connection using the ID (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the client.</param>
    /// <param name="ID">The ID of the connection to send the packet to.</param>
    /// <returns>True if an connection has been found and the sending the packet succeeded, otherwise false.</returns>
    public async ValueTask<bool> SendPacketAsync(Packet packet, byte[] ID) {
        if (Closed) { return false; }
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        return await SendPacketAsync(packet, connection);
    }

    /// <summary>
    /// Broadcasts a packet to all clients (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to broadcast.</param>
    /// <returns>True if the server is still running, otherwise false.</returns>
    public async ValueTask<bool> BroadcastPacketAsync(Packet packet) {
        if (Closed) { return false; }
        foreach (Connection connection in connections.Values) {
            await SendPacketAsync(packet, connection);
        }
        return true;
    }

    #endregion Send Asynchronous


    #region Disconnect

    /// <summary>
    /// Disconnects a connection using an ID.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    /// <returns>True if it succeeded, false if the server is closed or the server could not send the <see cref="DisconnectPacket"/>.</returns>
    public bool Disconnect(Connection connection, byte[]? additionalData = null) {
        if (Closed) { return false; }
        additionalData ??= [];
        if (!SendPacket(new DisconnectPacket(additionalData), connection)) return false;
        connections.Remove(connection.ID);
        connection.Dispose();
        OnDisconnect?.Invoke(this, connection, false, additionalData);
        return true;
    }

    /// <summary>
    /// Disconnects a connection using an ID.
    /// </summary>
    /// <param name="ID">The ID of the connection used to disconnect the client.</param>
    /// <returns>True if an connection has been found and sending the <see cref="DisconnectPacket"/> succeeded, otherwise false.</returns>
    public bool Disconnect(byte[] ID) {
        if (Closed) { return false; }
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        return Disconnect(connection);
    }

    /// <summary>
    /// Kicks or disconnects all clients from this server.
    /// </summary>
    /// <returns>True if the server is still running, otherwise false.</returns>
    public bool DisconnectAll() {
        if (Closed) { return false; }
        foreach (Connection connection in connections.Values) {
            Disconnect(connection);
        }
        return true;
    }

    #endregion Disconnect


    #region Disconnect Asynchronous

    /// <summary>
    /// Disconnects a connection using an ID (asynchronous).
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    /// <returns>True if an connection has been found and sending the <see cref="DisconnectPacket"/> succeeded, otherwise false.</returns>
    public async ValueTask<bool> DisconnectAsync(Connection connection, byte[]? additionalData = null) {
        if (Closed) { return false; }
        additionalData ??= [];
        if (!await SendPacketAsync(new DisconnectPacket(additionalData), connection)) {
            return false;
        }
        connections.Remove(connection.ID);
        connection.Dispose();
        OnDisconnect?.Invoke(this, connection, false, additionalData);
        return true;
    }

    /// <summary>
    /// Disconnects a connection using an ID (asynchronous).
    /// </summary>
    /// <param name="ID">The ID of the connection used to disconnect the client.</param>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    /// <returns>True if an connection has been found and sending the <see cref="DisconnectPacket"/> succeeded, otherwise false.</returns>
    public async ValueTask<bool> DisconnectAsync(byte[] ID, byte[]? additionalData = null) {
        if (Closed) { return false; }
        if (!connections.TryGetValue(ID, out Connection? connection)) {
            return false;
        }
        return await DisconnectAsync(connection, additionalData);
    }

    /// <summary>
    /// Kicks or disconnects all clients from this server (asynchronous).
    /// </summary>
    /// <param name="additionalData">The additional data for disconnection (default: empty).</param>
    /// <returns>True if the server is still running, otherwise false.</returns>
    public async ValueTask<bool> DisconnectAllAsync(byte[]? additionalData = null) {
        if (Closed) { return false; }
        foreach (Connection connection in connections.Values) {
            await DisconnectAsync(connection, additionalData);
        }
        return true;
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
            if (!connection.Parser.Parse(GlobalFormatter, () => Closed || connection.Closed, p => {
                OnPacket?.Invoke(this, connection, p);
                if (p is DisconnectPacket) {
                    connections.Remove(connection.ID);
                    connection.Dispose();
                    OnDisconnect?.Invoke(this, connection, true, (p as DisconnectPacket)!.additionalData);
                    return true;
                }
                return false;
            })) {
                string message = $"Unable to start reading from network stream (connection ID: {connection.ToConnectionID()}).";
                Logger?.LogFatal(message);
                OnException?.Invoke(new InvalidOperationException(message), connection);
            }
        } catch (IOException e) {
            if (Closed || connection.Closed) {
                return;
            }
            Logger?.LogWarning($"Could not read from network stream (connection ID: {connection.ToConnectionID()}):");
            Logger?.LogException(e, Severity.Warning);
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