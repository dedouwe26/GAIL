using System.Net;
using System.Net.Sockets;
using GAIL.Networking.Parser;
using OxDED.Terminal;
using OxDED.Terminal.Logging;

namespace GAIL.Networking.Client;

/// <summary>
/// This is a client that creates a connection to the server.
/// </summary>
public class ClientContainer : IDisposable {
    /// <summary>
    /// Creates a client.
    /// </summary>
    /// <param name="server">The server end point (IP address and port).</param>
    /// <param name="local">The local end point (for different port, etc).</param>
    /// <param name="enableLogging">If the client should start logging.</param>
    /// <param name="logger">The logger to use (default: ID: GAIL.Networking.Client.{ Connection.CreateID( localEndpoint ) } ).</param>
    /// <returns>The created client, if it didn't fail.</returns>
    public static ClientContainer? Create(IPEndPoint server, IPEndPoint? local = null, bool enableLogging = false, Logger? logger = null) {
        ClientContainer client;
        try {
            client = new(server, local, enableLogging, logger);
        } catch (SocketException) {
            return null;
        }
        
        return client;
    }
    /// <summary>
    /// The server end point (IP address and port).
    /// </summary>
    public readonly IPEndPoint Server;
    /// <summary>
    /// The local end point or the end point the server sees (IP address and port, for different port or different IP address).
    /// </summary>
    public readonly IPEndPoint IP;
    private Thread? listenThread;
    /// <summary>
    /// The logger of this client.
    /// Set it using <see cref="SetLogger"/>
    /// </summary>
    public Logger? Logger { get; private set; }
    /// <summary>
    /// The tcp client back-end.
    /// </summary>
    public readonly TcpClient tcpClient;
    /// <summary>
    /// The stream to write to.
    /// </summary>
    public NetworkStream? NetworkStream {get; private set;}
    /// <summary>
    /// An event that is called when a packet is received.
    /// </summary>
    public event PacketCallback? OnPacket;
    /// <summary>
    /// An event that is called when a connection is established.
    /// </summary>
    public event ConnectCallback? OnConnect;
    /// <summary>
    /// An event that is called when the client has stopped.
    /// </summary>
    public event StopCallback? OnStop;
    /// <summary>
    /// An event that is called when the client is disconnecting.
    /// </summary>
    public event DisconnectCallback? OnDisconnect;
    /// <summary>
    /// An event that is called when a packet is sent.
    /// </summary>
    public event PacketSentCallback? OnPacketSent;
    /// <summary>
    /// An event for when an exception is thrown (only IOException, SocketException).
    /// </summary>
    public event ExceptionCallback? OnException;
    /// <summary>
    /// If the connection is closed.
    /// </summary>
    public bool Closed {get; private set;}
    /// <exception cref="SocketException"/>
    private ClientContainer(IPEndPoint server, IPEndPoint? local, bool enableLogging = false, Logger? logger = null) {
        Server = server;
        Closed = true;
        try {
            if (local == null) {
                tcpClient = new TcpClient();
            } else {
                tcpClient = new TcpClient(local);
            }

            IP = (tcpClient.Client.LocalEndPoint! as IPEndPoint)!;
            if (enableLogging) {
                SetLogger(logger);
            }
        } catch (SocketException e) {
            logger?.LogFatal("Unable to initiate the socket: '"+e.Message+"'");
            throw;
        }
    }

    /// <summary>
    /// Disposes the client.
    /// </summary>
    ~ClientContainer() {
        Dispose();
    }
    /// <summary>
    /// Sets the logger of this client.
    /// </summary>
    /// <param name="logger">The logger for this client ( default: ID: GAIL.Networking.Client.{ Connection.CreateID( localEndpoint ) } ).</param>
    /// <param name="disable">If it should disable the logging.</param>
    public void SetLogger(Logger? logger = null, bool disable = false) {
        Logger = disable ? null : (logger ?? new Logger(
            $"GAIL.Networking.Client.{Networking.Server.Connection.CreateID(IP)}",
            "Networking Client",
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

    /// <summary>
    /// Starts listening (asynchronous).
    /// </summary>
    /// <returns>True if connected successfully.</returns>
    public async ValueTask<bool> StartAsync() {
        try {
            await tcpClient.ConnectAsync(Server);
        } catch (SocketException e) {
            Logger?.LogFatal("Unable to connect to server: '"+e.Message+"'.");
            OnException?.Invoke(e);
            return false;
        }
        NetworkStream = tcpClient.GetStream();
        Closed = false;
        OnConnect?.Invoke(this);
        listenThread = new Thread(Listen);
        listenThread.Start();
        return true;
    }
    /// <summary>
    /// Starts listening.
    /// </summary>
    /// <returns>True if connected successfully.</returns>
    public bool Start() {
        try {
            tcpClient.Connect(Server);
        } catch (SocketException e) {
            Logger?.LogFatal("Unable to connect to server: '"+e.Message+"'.");
            OnException?.Invoke(e);
            return false;
        }
        NetworkStream = tcpClient.GetStream();
        Closed = false;
        OnConnect?.Invoke(this);
        listenThread = new Thread(Listen);
        listenThread.Start();
        return true;
    }
    private void Listen() {
        try {
            if (!NetworkParser.Parse(NetworkStream!, () => Closed, (Packet p) => {
                OnPacket?.Invoke(this, p);
                if (p is DisconnectPacket dp) {
                    OnDisconnect?.Invoke(this, true, dp.AdditionalData);
                    Dispose();
                    return true;
                }
                return false;
            })) {
                Logger?.LogFatal("Unable to start reading from network stream.");
                OnException?.Invoke(new InvalidOperationException("Unable to start reading from network stream."));
            }
        } catch (IOException e) {
            if (Closed) {
                return;
            }
            Logger?.LogError("Could not read from network stream: '"+e.Message+"'.");
            OnException?.Invoke(e);
        }
        
    }
    /// <summary>
    /// Sends a packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    public void SendPacket(Packet packet) {
        if (Closed) { return; }
        try {
            NetworkParser.Serialize(NetworkStream!, packet);
        } catch (IOException e) {
            Logger?.LogError("Could not send packet: '"+e.Message+"'.");
            OnException?.Invoke(e);
        }
        NetworkStream!.Flush();
        OnPacketSent?.Invoke(this, packet);
    }

    /// <summary>
    /// Sends a packet to the server (asynchronous).
    /// </summary>
    /// <param name="packet">The packet to send to the server.</param>
    public async ValueTask SendPacketAsync(Packet packet) {
        if (Closed) { return; }
        try {
            NetworkParser.Serialize(NetworkStream!, packet); // TODO?: this isnt really async.
        } catch (IOException e) {
            Logger?.LogError("Could not send packet: '"+e.Message+"'.");
            OnException?.Invoke(e);
        }
        await NetworkStream!.FlushAsync();
        OnPacketSent?.Invoke(this, packet);
    }

    /// <summary>
    /// Disconnects from the server and disposes everything.
    /// </summary>
    /// <param name="additionalData">The optional additional data.</param>
    public void Stop(byte[]? additionalData = null) {
        if (Closed) { return; }
        additionalData ??= [];
        OnDisconnect?.Invoke(this, false, additionalData);
        SendPacket(new DisconnectPacket(additionalData));
        Dispose();
    }
    /// <summary>
    /// Disconnects from the server and disposes everything (asynchronous).
    /// </summary>
    /// <param name="additionalData">The optional additional data send to the server.</param>
    public async ValueTask StopAsync(byte[]? additionalData = null) {
        if (Closed) { return; }
        Logger?.LogDebug("Stopping");
        additionalData ??= [];
        OnDisconnect?.Invoke(this, false, additionalData);
        await SendPacketAsync(new DisconnectPacket(additionalData ?? []));
        Dispose();
    }
    /// <inheritdoc/>
    /// <remarks>
    /// <b>Please use </b><see cref="Stop"/><para/>
    /// <b>Will not call the ClientContainer.OnDisconnect event and will not send a </b><see cref="DisconnectPacket"/>.<para/>
    /// Disconnects from the server and disposes everything.
    /// </remarks>
    public void Dispose() {
        if (Closed) { return; }
        Logger?.LogDebug("Disposing");
        Closed = true;
        NetworkStream!.Close();
        tcpClient.Close();
        OnStop?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}